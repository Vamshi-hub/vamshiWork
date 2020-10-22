import { Component } from '@angular/core';
import { Location } from '@angular/common';
import * as pbi from 'powerbi-client';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../material-track.service';
import { ProjectMaster } from '../classes/project-master';
import { JwtHelper } from '../../shared/classes/jwt-helper';
import { AccessToken } from '../../shared/classes/access-token';

@Component({
  selector: 'app-powerbi-viewer',
  templateUrl: './powerbi-viewer.component.html',
  styleUrls: ['./powerbi-viewer.component.css']
})
export class PowerbiViewerComponent extends CommonLoadingComponent {

  powerbi: pbi.service.Service;
  reportContainer: HTMLElement;
  report_guid: string;
  project: ProjectMaster;

  constructor(route: ActivatedRoute, router: Router, private location: Location,
    private uiUtilService: UiUtilsService,
    private materialTrackService: MaterialTrackService) {

    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.reportContainer = <HTMLElement>document.getElementById('reportContainer');
    this.powerbi = new pbi.service.Service(pbi.factories.hpmFactory, pbi.factories.wpmpFactory, pbi.factories.routerFactory);

    this.route.params.subscribe(params => {
      this.powerbi.reset(this.reportContainer);

      if (!this.isLoading)
        this.isLoading = true;

      this.report_guid = params['guid'];
      this.route.parent.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
        this.project = await (data.project);
        this.initEmbedding();
      });
    });
  }

  initEmbedding() {
    this.materialTrackService.getPowerBIToken().subscribe(token => {
      if (token) {
        this.materialTrackService.getPowerBIEmbedToken(token, this.report_guid).subscribe(embed_token => {

          if (embed_token) {

            this.isLoading = false;

            let models = pbi.models;
            let embedConfiguration = {
              type: 'report',
              id: this.report_guid,
              embedUrl: 'https://app.powerbi.com/reportEmbed',
              tokenType: models.TokenType.Embed,
              accessToken: embed_token.token,
              settings: {
                filterPaneEnabled: false
              }
            } as pbi.IEmbedConfiguration;

            let filters = [];
            if (this.project) {
              filters.push({
                $schema: "http://powerbi.com/product/schema#basic",
                target: {
                  table: "ProjectMaster",
                  column: "ID"
                },
                operator: "In",
                values: [this.project.id],
                filterType: pbi.models.FilterType.Basic
              } as pbi.models.IBasicFilter);
            }
            const accessTokenStr = window.localStorage.getItem('access_token');
            if (accessTokenStr) {
              const accessToken = JwtHelper.decodeToken(accessTokenStr) as AccessToken;

              if (accessToken.vendorId > 0) {
                filters.push({
                  $schema: "http://powerbi.com/product/schema#basic",
                  target: {
                    table: "VendorMaster",
                    column: "ID"
                  },
                  operator: "In",
                  values: [accessToken.vendorId],
                  filterType: pbi.models.FilterType.Basic
                } as pbi.models.IBasicFilter);
              }
            }
            embedConfiguration.filters = filters;
            
            let report = this.powerbi.embed(this.reportContainer, embedConfiguration);
            // Report.on will add an event handler which prints to Log window.
            report.on("loaded", function () {
              console.log("Loaded");
            });
          }
          else {
            this.isLoading = false;
            this.uiUtilService.openSnackBar('Fail to get report', 'OK');
          }
        });
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('Fail to validate with Power BI', 'OK');
      }
    }, error => {
      this.isLoading = false;
      this.uiUtilService.openSnackBar(error, 'OK');
    });

  }

  onBackButtonClicked(): void {
    this.location.back();
  }
}
