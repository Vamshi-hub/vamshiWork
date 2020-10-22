import { Component, OnInit, Renderer2 } from '@angular/core';
import { ProjectMaster } from '../material-track/classes/project-master';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../shared/authentication.service';
import { UiUtilsService } from '../shared/ui-utils.service';
import { AccessToken } from '../shared/classes/access-token';
import { JwtHelper } from '../shared/classes/jwt-helper';
import { TenantSettings } from '../shared/classes/tenant_settings';

@Component({
  selector: 'app-top-menu',
  templateUrl: './top-menu.component.html',
  styleUrls: ['./top-menu.component.css']
})
export class TopMenuComponent implements OnInit {
  showProjects: boolean;
  showMenu: boolean;
  projects: ProjectMaster[];
  selectedProject: ProjectMaster;
  personName: string;
  blocks: string[];
  errorMessage: string;
  tenantSettings: TenantSettings;
  logoImageUrl: string;
  materialTracking = false;
  jobTracking = false;
  
  constructor(private route: ActivatedRoute, private router: Router,
    private authentictionService: AuthenticationService,
    private uiUtilService: UiUtilsService, private renderer: Renderer2) {
  }

  ngOnInit() {
    this.showProjects = false;
    this.showMenu = true;
    this.errorMessage = null;
    this.setLogo();
    
    this.route.url.subscribe(segments => {
      if (segments[0].path === 'material-tracking' || segments[0].path === 'job-tracking') {
        this.route.data.subscribe(async (data: { projects: ProjectMaster[] }) => {
          if (await data.projects) {
            this.showProjects = true;
            this.projects = data.projects;
            
            // if (this.projects.length === 1) {
            //   console.log("Project ID from Top Menu: " + this.projects[0].id);
            //   window.localStorage.setItem("project_id", String(this.projects[0].id));
            //   //this.showProjects = false;
            // }
            
            const projectId = +window.localStorage.getItem("project_id");
            if (projectId) {
              this.selectedProject = this.projects.find(p => p.id == +projectId);
              this.uiUtilService.closeAllDialog();
            }
            else {
              this.showMenu = false;
              this.errorMessage = "Please select a project";
              this.uiUtilService.closeAllDialog();
            }
          }
          else {
            this.uiUtilService.closeAllDialog();
            this.errorMessage = "No projects found";
          }
        });

      }
    });

    const accessTokenStr = window.localStorage.getItem('access_token');
    if (accessTokenStr) {
      const accessToken = JwtHelper.decodeToken(accessTokenStr) as AccessToken;
      this.personName = accessToken.personName;
    }
  }

  onRouterActivate(event){
  }

  onRouterDeactivate(event){    
  }

  onLogoutClicked() {
    this.authentictionService.logOut();
  }

  onProfileClicked() {
    let id = window.localStorage.getItem("user_id");
    this.router.navigateByUrl('/user-account/' + id);
  }

  changePassword() {
    this.uiUtilService.openSnackBar("The page is under construction", "OK");
    //this.router.('/material-tracking/User/changePassword' + [this.personName]);   
  }

  onProjectSelected(project: ProjectMaster) {
    this.selectedProject = project;
    //this.errorMessage = null;
    window.localStorage.setItem("project_id", String(project.id));
    window.localStorage.removeItem('blk');
    var accessTokenStr = window.localStorage.getItem("access_token");
    var accessToken = JwtHelper.decodeToken(accessTokenStr) as AccessToken;
    //this.router.navigateByUrl(accessToken.role.defaultPage);
    window.location.reload();
  }

  onMenuButtonClicked() {
    window.scrollTo(0, 0);
  }

  setLogo() {
    const tenantSettingsStr = window.localStorage.getItem('tenant_settings');
    if (tenantSettingsStr) {
      this.tenantSettings = JSON.parse(tenantSettingsStr) as TenantSettings;
      if (this.tenantSettings.logoImageURL)
          this.logoImageUrl = this.tenantSettings.logoImageURL;
          console.log(this.tenantSettings.enabledModules);
      if(this.tenantSettings.enabledModules)
      {
        console.log(this.tenantSettings.enabledModules);
          this.materialTracking = this.tenantSettings.enabledModules.indexOf("material") != -1 ? true : false;
          this.jobTracking = this.tenantSettings.enabledModules.indexOf("job") != -1 ? true : false;
      }
    }
  }
}