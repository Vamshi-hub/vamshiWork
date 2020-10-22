import { Component, OnInit } from '@angular/core';
import { SiteMaster } from '../classes/site-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ConfigurationService } from '../configuration.service';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { ProjectMaster } from '../../material-track/classes/project-master';
import { MatSelectChange } from '@angular/material';
import { NotificationTimer } from '../classes/notification-timer';
import { OrganisationMaster } from '../../material-track/classes/organisation-master';
import * as moment from 'moment';

@Component({
  selector: 'app-notification-config',
  templateUrl: './notification-config.component.html',
  styleUrls: ['./notification-config.component.css']
})
export class NotificationConfigComponent extends CommonLoadingComponent {
  selectedNotificationfor = "project";
  sites: SiteMaster[];
  organisationSites: SiteMaster[];
  organisations: OrganisationMaster[];
  projects: ProjectMaster[];
  lstNotification: NotificationTimer[];
  selectedProject: number;
  selectedSite: number;
  selectedOrganisation: number;
  showConfig = false;
  showProject = true;
  checked = true;
  VsibleNotification = false;
  //private config = { hour: 7, minute: 15, meriden: 'PM', format: 12 };


  public steps: any = { hour: 1, minute: 5 };
  public value: Date = new Date(2000, 2, 0, 0, 0);

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private configurationService: ConfigurationService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.getProjectsOrsites();
  }

  onProjectSelected(event: MatSelectChange) {
    this.getNotifications(event.value, 'project');
  }

  onSiteSelected(event: MatSelectChange) {
    this.getNotifications(event.value, 'site');
  }
  onOrganisationSelected(event: MatSelectChange) {
    this.organisationSites = this.sites.filter(s => s.organisationID == event.value);
    this.VsibleNotification = false;
  }
  onNotificationForChanged(event: MatSelectChange) {
    if (event.value == 'project')
      this.showProject = true;
    else
      this.showProject = false;
    this.selectedProject = 0;
    this.selectedSite = 0;
    this.selectedOrganisation = 0;
    this.VsibleNotification = false;
    this.getProjectsOrsites();
  }

  getProjectsOrsites() {
    if (this.selectedNotificationfor == 'project' && this.projects == null) {
      this.materialTrackService.getListProjects().subscribe(
        data => {
          this.projects = data.filter(p => p.country.length > 0);
          this.isLoading = false;
        },
        error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, "OK");
        });
    }
    else if (this.sites == null) {
      this.configurationService.getSites().subscribe(
        data => {
          this.sites = data.filter(s => s.organisationID != 0);
          this.organisations = Array<OrganisationMaster>();
          data.forEach(item => {
            if (item.organisationID != 0 && !this.organisations.find(v => v.id == item.organisationID)) {
              let organisation: OrganisationMaster = new OrganisationMaster();
              organisation.id = item.organisationID;
              organisation.name = item.organisationName;

              this.organisations.push(organisation);
            }
          })
          this.isLoading = false;
        },
        error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, "OK");
        });
    }
  }

  getNotifications(id: number, type: string) {
    this.isLoading = true;
    this.VsibleNotification = false;
    this.configurationService.getNotifications(id, type).subscribe(
      data => {
        this.lstNotification = data;
        for (let item of this.lstNotification) {
          item.timer = moment.utc(item.triggerTime, "HH:mm");
        }

        this.isLoading = false;
        this.VsibleNotification = true;
      },
      error => {
        console.log(error);
        this.isLoading = false;
        this.VsibleNotification = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  OnSave() {
    this.isLoading = true;
    this.VsibleNotification = false;
    for (let item of this.lstNotification) {
      item.triggerTime = moment(item.timer, 'hh:mm a').format("HH:mm");
    }
    this.lstNotification.forEach(item => {
      console.log(item.timer)
    })
    this.configurationService.postNotification(this.lstNotification).subscribe(
      result => {
        if (result["lstnotifications"]) {
          let notificationresponse: NotificationTimer[];
          notificationresponse = result["lstnotifications"] as NotificationTimer[];
          this.lstNotification.forEach(item => {
            item.id = notificationresponse.filter(n => n.code == item.code)[0].id;
          })
          this.VsibleNotification = true;
          this.uiUtilService.openSnackBar("Saved Successfully", "OK");
        }
        this.isLoading = false;
      },
      error => {
        console.log(error);
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    )
  }
  OnReset() {
    if (this.showProject)
      this.getNotifications(this.selectedProject, 'project');
    else {
      this.getNotifications(this.selectedSite, 'site');
    }
  }
}
