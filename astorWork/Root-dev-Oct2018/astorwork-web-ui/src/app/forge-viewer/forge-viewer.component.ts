import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { UiUtilsService } from '../shared/ui-utils.service';
import { CommonLoadingComponent } from '../shared/common-loading/common-loading.component';
import { ActivatedRoute, Router } from '@angular/router';
import { MaterialTrackService } from '../material-track/material-track.service';
import * as moment from 'moment';
import { BIMViewerProgress } from '../material-track/classes/bim-viewer-progress';

declare let THREE: any;

@Component({
  selector: 'app-forge-viewer',
  templateUrl: './forge-viewer.component.html',
  styleUrls: ['./forge-viewer.component.css']
})
export class ForgeViewerComponent extends CommonLoadingComponent {
  modelURN: string;
  progress: BIMViewerProgress[];
  showProgress = false;
  elementId = 0;
  errorMessage: string;
  forgeToken: string;
  doc: any;
  viewerApp: any;
  viewables: any;
  viewer: any;
  model: any;
  indexViewable = 0;

  constructor(route: ActivatedRoute, router: Router, private location: Location,
    private uiUtilService: UiUtilsService, private mtService: MaterialTrackService) {
    super(route, router);
  }

  ngOnDestroy() {
    if (this.viewerApp) {
      this.viewerApp.finish();
    }
  }

  ngAfterViewInit() {
    this.modelURN = sessionStorage.getItem('model_urn');
    let elementIdStr = sessionStorage.getItem('element_id');
    let progressStr = sessionStorage.getItem(`${this.modelURN}_progress`);
    if (elementIdStr) {
      this.elementId = parseInt(elementIdStr);
    }

    if (this.modelURN) {
      if (progressStr)
        this.progress = JSON.parse(progressStr) as BIMViewerProgress[];
      else {
        this.mtService.getForgeModelProgress(this.modelURN).subscribe(result => {
          this.progress = result;
          sessionStorage.setItem(`${this.modelURN}_progress`, JSON.stringify(result));
        }, error => {
          this.uiUtilService.openSnackBar(error, "OK");
        });
      }

      var tokenExpireStr = sessionStorage.getItem("forge_token_expire_time");
      if (tokenExpireStr) {
        var tokenExpireTime = moment(tokenExpireStr);
        var now = moment();
        if (tokenExpireTime > now.add(5, 'minutes')) {
          this.forgeToken = sessionStorage.getItem('forge_token');
        }
        else {
          console.log('forge token expired');
        }
      }

      this.uiUtilService.loadScript(
        'https://developer.api.autodesk.com/modelderivative/v2/viewers/viewer3D.min.js'
      ).then((value: Event) => {
        this.uiUtilService.loadScript('./assets/javascripts/astoria_measure.js').then((value: Event) => {
          if (this.forgeToken) {
            this.initViewer();
          }
          else {
            this.mtService.getForgeToken().subscribe(data => {
              this.forgeToken = data.access_token;
              sessionStorage.setItem('forge_token', this.forgeToken);
              sessionStorage.setItem("forge_token_expire_time",
                moment().add(data.expires_in, "seconds").format());
              this.initViewer();
            }, error => {
              this.isLoading = false;
              this.errorMessage = error;
            });
          }
        });
      }, (reason: Event) => {
        this.isLoading = false;
        console.log(reason);
        this.errorMessage = 'Fail to load forge viewer';
      });
    }
    else {
      this.errorMessage = 'No BIM model';
      this.isLoading = false;
    }
  }

  ngOnInit() {
    super.ngOnInit();
  }

  initViewer() {
    const options = {
      env: 'AutodeskProduction',
      accessToken: this.forgeToken,
      useADP: false
    };

    Autodesk.Viewing.Initializer(options, () => this.onInitialized());
  }

  onInitialized(): void {
    // const documentId = 'dXJuOmFkc2sub2JqZWN0czpvcy5vYmplY3Q6YXN0b3J3b3JrLWNvbnZlcnQvc291cmNlXzIwMTcxMjE4Lm53ZA';
    const documentId = btoa(this.modelURN);

    // this.forgeService.getPropertyDb(this.forgeToken, documentId);

    this.viewerApp = new Autodesk.Viewing.ViewingApplication('viewerContainer');
    const config3d = {
      startOnInitialize: false,
      extensions: ['Autodesk.Measure']
      //extensions: ['Autodesk.Measure', 'Astoria.ForgeExtension']
    };

    // this.viewerApp.registerViewer(this.viewerApp.k3D, Autodesk.Viewing.Private.GuiViewer3D, config3d);
    this.viewerApp.registerViewer(this.viewerApp.k3D, Autodesk.Viewing.Viewer3D, config3d);

    this.viewerApp.loadDocument(
      'urn:' + documentId,
      (doc) => this.onDocumentLoadSuccess(doc),
      (viewerErrorCode) => this.onDocumentLoadFailure(viewerErrorCode)
    );
  }

  onDocumentLoadSuccess(doc): void {
    this.doc = doc;
    // A document contains references to 3D and 2D viewables.
    this.viewables = this.viewerApp.bubble.search({ 'type': 'geometry' });
    if (this.viewables.length === 0) {
      console.error('Document contains no viewables.');
    } else {
      // Choose any of the avialble viewables
      const initialViewable = this.viewables[this.indexViewable];
      const svfUrl = doc.getViewablePath(initialViewable);
      const modelOptions = {
        sharedPropertyDbPath: doc.getPropertyDbPath()
      };

      // Choose any of the avialble viewables
      this.viewerApp.selectItem(this.viewables[this.indexViewable].data,
        (viewer, item) => this.onItemLoadSuccess(viewer, item),
        (errorCode) => this.onItemLoadFail(errorCode)
      );
    }
  }

  onDocumentLoadFailure(viewerErrorCode): void {
    this.isLoading = false;
    console.error('onDocumentLoadFailure() - errorCode:' + viewerErrorCode);
    this.errorMessage = 'Load BIM model failed';
  }

  onItemLoadSuccess(viewer, item): void {
    console.log('onItemLoadSuccess()!');
    // Congratulations! The viewer is now ready to be used.
    // console.log('Viewers are equal: ' + (viewer === this.viewerApp.getCurrentViewer()));
    // viewer.addEventListener(Autodesk.Viewing.SELECTION_CHANGED_EVENT, this.onSelectionChanged);

    this.viewer = viewer;
    const onSelectionBinded = this.onModelLoaded.bind(this);
    this.viewer.addEventListener(Autodesk.Viewing.GEOMETRY_LOADED_EVENT, onSelectionBinded);
    //var measureExtension = viewer.getExtension('Autodesk.Measure');
    //console.log(measureExtension.measureTool.getName());
  }

  onItemLoadFail(errorCode): void {
    this.isLoading = false;
    console.error('onItemLoadFail() - errorCode:' + errorCode);
    this.errorMessage = 'Load BIM model failed';
  }

  onModelLoaded(data): void {
    this.model = data.model;
    if (this.elementId > 0) {
      this.viewer.isolateById(this.elementId);
      this.viewer.fitToView([this.elementId]);
    }
    else {
      this.viewer.fitToView();
    }
    this.viewer.setBackgroundColor(0, 0, 0, 70, 75, 189);
    this.viewer.setEnvMapBackground(false);
    this.viewer.setGroundReflection(false);
    this.viewer.setGhosting(true);
    if (sessionStorage.getItem('low_quality')) {
      this.viewer.setQualityLevel(false, false);
      this.viewer.setGroundShadow(false);
    }
    else {
      this.viewer.setQualityLevel(true, true);
      this.viewer.setGroundShadow(true);
    }
    this.viewer.setProgressiveRendering(true);
    this.viewer.prefs.tag('ignore-producer');
    this.viewer.run();
    this.isLoading = false;
  }

  onToggleIsolate(): void {
    if (this.viewer.areAllVisible())
      this.viewer.isolateById(this.elementId);
    else {
      this.viewer.showAll();
    }
  }

  onToggleProgress(): void {
    this.showProgress = !this.showProgress;
    if (this.showProgress) {
      this.viewer.setBackgroundColor(255, 255, 255, 255, 255, 255);
      let qcFailColor = new THREE.Vector4(1, 0, 0, 1);
      for (let element of this.progress) {
        if (element.passed)
          this.viewer.setThemingColor(element.elementId, this.colorToVector4(element.stageColour));
        else
          this.viewer.setThemingColor(element.elementId, qcFailColor);
      }
    }
    else {
      this.viewer.clearThemingColors();
      this.viewer.setBackgroundColor(0, 0, 0, 70, 75, 189);
    }
  }

  colorToVector4(color: string) {
    let r = parseInt(color.substr(1, 2), 16);
    let g = parseInt(color.substr(3, 2), 16);
    let b = parseInt(color.substr(5, 2), 16);
    return new THREE.Vector4(r / 255, g / 255, b / 255, 1);
  }

  onZoomInButtonClicked(): void {
    //this.viewer.isolateById(this.elementId);
    this.viewer.fitToView([this.elementId]);
  }

  onZoomOutButtonClicked(): void {
    //this.viewer.showAll();
    this.viewer.fitToView([this.model.getRootId()]);
  }

  onBackButtonClicked(): void {
    this.location.back();
  }
}
