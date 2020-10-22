import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-common-loading',
  templateUrl: './common-loading.component.html',
  styleUrls: ['./common-loading.component.css']
})
export class CommonLoadingComponent implements OnInit {
  isLoading: boolean;
  errorMessage: string;
  accessRight = 0;
  route: ActivatedRoute;
  router: Router;

  ngOnInit() {
    this.isLoading = true;
    
    this.route.data.subscribe(async (data: { accessRight: number }) => {
      this.accessRight = await (data.accessRight);
    });
  }

  constructor(route: ActivatedRoute, router: Router) {
    this.route = route;
    this.router = router;
  }
}
