import { Component, OnInit, ViewChild, ComponentFactoryResolver, TemplateRef } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator, MatSelectChange } from '@angular/material';
import { Mrf } from '../classes/mrf-master';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../material-track.service';
import { ProjectMaster } from '../classes/project-master';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { filter } from 'rxjs/operators';
import { PrintQRItem } from '../classes/print-qr-item';
import { QRCode } from '../classes/qrCode';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
@Component({
  selector: 'app-list-mrf',
  templateUrl: './list-mrf.component.html',
  styleUrls: ['./list-mrf.component.css']
})
export class ListMrfComponent extends CommonLoadingComponent {
  displayedColumns = ['mrfNo', 'block', 'level', 'zone', 'materialTypes', 'organisationName', 'orderDate', 'expectedDeliveryDate', 'progress', 'print'];
  dataSource: MatTableDataSource<Mrf> = new MatTableDataSource();
  project: ProjectMaster;
  MrfDetails:Mrf;
  blocks = [];
  qrCodes: PrintQRItem[];
  selectedBlock = 'all';
  filter = '';
  @ViewChild('modalContent')
  modalContent: TemplateRef<any>;
  modalData: {
    action: string;
    event: Mrf;
  };
  Math: any;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(route: ActivatedRoute, router: Router,
    private cfResover: ComponentFactoryResolver,
    private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService,private modal: NgbModal,) {
    super(route, router);
  }
  

  ngOnInit() {
    super.ngOnInit();

    this.Math = Math;
    this.dataSource = new MatTableDataSource();
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      await this.route.paramMap.subscribe((params: Params) => {
        this.filter = params.get('filter') == null ? '' : params.get('filter');
      });

      const project = await (data.project);
      if (project) {
        this.project = project;
        this.materialTrackService.getProjectInfo(this.project.id, "").subscribe(
          data => {
            this.blocks=data.blocks;
          }
        );
        this.getMRFsByBlk();
      }
      else {
        this.uiUtilService.openSnackBar('No projects found', 'OK');
        this.isLoading = false;
      }

    });
  }
  
  onBlockChanged(event: MatSelectChange) {
    if (event.value == 'all') {
      this.getMRFsByBlk();
    }
    else {
      this.getMRFsByBlk(event.value);
    }
  }

  selectRow(row: any) {
    this.router.navigate(['../materials', { blk: row.block, mrfNo: row.mrfNo }], { relativeTo: this.route })
  }

  onFilterKeyIn(event: KeyboardEvent) {
    var filterValue = (<HTMLInputElement>event.target).value;
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  applyFilteToDataSource(){
    let message = '';
   
        if (this.dataSource.data.length > 0) {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.dataSource.filterPredicate = (data: Mrf, filter: string) => {
            const filterArray = filter.split(' ');
            let countMatch = 0;
            for (let entry of filterArray) {
              if (data.level.toLowerCase().indexOf(entry) >= 0 ||
                data.block.toLowerCase().indexOf(entry) >= 0 ||
                data.zone.toLowerCase().indexOf(entry) >= 0 ||
                data.organisationName.toLowerCase().indexOf(entry) >= 0 ||
                data.mrfNo.toLowerCase().indexOf(entry) >= 0
               )
                countMatch++;
            }
            if (countMatch == filterArray.length)
              return true;
            else
              return false;
          };
          this.filter = this.filter.toLowerCase();
          this.dataSource.filter = this.filter;
        }
        
  }
  onPrintClicked(event: Event, mrfId: number) {
    this.isLoading = true;
    event.stopPropagation();
    /*
    let item = new PrintQRItem();
    item.tag = '123456';
    item.projectManagerName = 'Mich';
    item.vendorName = 'PPVC manufacturer';
    item.materialType = 'PPVC-Kitchen';
    item.markingNo = 'A27';
    this.qrCodes = [item, item, item];
    */
    this.isLoading = true;
    this.materialTrackService.getListQRCodeByMRF(this.project.id, mrfId).subscribe(
      data => {
        this.isLoading = false;
        this.qrCodes = data;
        setTimeout(this.printQRCode, 1000);
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
        this.isLoading = false;
      }
    );
    /*
     var WinPrint = window.open('', '', 'left=0,top=0,width=400,height=240,toolbar=0,scrollbars=0,status=0');
     WinPrint.document.write('<img src="/assets/svg/qr-code.svg" alt="Sample QR codel" width="200" height="200">');
     WinPrint.document.write('<div style="text-align: center; position: absolute; left: 210px; top: 0;"><h2>S1S6C</h2>');
     WinPrint.document.write('<p>Manager : Charlie Murphy<br>\
     Vendor : Reinforcing Asia<br>\
     Type :Slab</p></div>');
     */
    /*
    WinPrint.document.close();
    WinPrint.focus();
    */
  }

  printQRCode() {
    var html = document.getElementById('printContainer').innerHTML;
    if (html.length > 0) {      
      var winPrint = window.open('', '', 'left=0,top=0,width=400,height=800,toolbar=0,scrollbars=0,status=0');
      winPrint.document.write(
        `<style>
          body, html { background-color: white; font-family: "Lucida Sans Unicode", "Lucida Grande", sans-serif; margin: 0; }
          img { width: 160px; }
          .has-margin { margin-bottom: 100px;}
          dl { margin: 0 20px 0 0; }
          dt { font-size: 26px; }
        </style>`);
      winPrint.document.write(html);
      winPrint.document.close();
      //winPrint.print();
      //winPrint.close();
    }
    else {
      setTimeout(this.printQRCode, 1000);
    }
  }

  getMRFsByBlk(blk?: string) {
    this.materialTrackService.getListMRFs(this.project.id, blk).subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {
          setTimeout(()=>{
            this.dataSource.paginator=this.paginator;
            this.dataSource.sort=this.sort;
          });
          //this.applyFilteToDataSource();
        }
        else {
          this.uiUtilService.openSnackBar("This project doesn't have any MRFs", 'OK');
        }
        this.isLoading = false;
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
        this.isLoading = false;
      });
  }
  showmrfdetails(event,element){
    event.stopPropagation(); 
   this.handleEvent('',event);
   this.MrfDetails=element
  }
  handleEvent(action: string, event: Mrf): void {
    this.modalData = { event, action };
    this.modal.open(this.modalContent, { size: 'lg' });
    
  }
}
