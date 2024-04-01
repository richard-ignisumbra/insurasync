import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import { Component, Input, OnInit } from "@angular/core";
import { MatDrawer, MatDrawerMode } from "@angular/material/sidenav";

import { Application } from "../../../../core/api/v1/model/application";
import { ApplicationSection } from "../../../../core/api/v1/model/applicationSection";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { HttpClient } from "@angular/common/http";
import { LayoutService } from "../../../../../@vex/services/layout.service";
import { Observable } from "rxjs";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import { map } from "rxjs/operators";
import { protectedResources } from "../../../../auth-config";
import { stagger40ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "application-side-menu",
  templateUrl: "./application-side-menu.component.html",
  styleUrls: ["./application-side-menu.component.scss"],
  animations: [stagger40ms, fadeInUp400ms],
})
export class ApplicationSideMenuComponent implements OnInit {
  applicationsEndpoint: string = protectedResources.applicationsApi.endpoint;
  applicationDetails: Application;
  ColumnMode = ColumnMode;
  isDesktop$ = this.layoutService.isDesktop$;

  private parametersSubscription: any;

  @Input() drawer: MatDrawer;
  @Input() applicationSections: ApplicationSection;

  drawerMode$: Observable<MatDrawerMode> = this.isDesktop$.pipe(
    map((isDesktop) => (isDesktop ? "side" : "over"))
  );
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private applicationsService: ApplicationsService,
    private layoutService: LayoutService
  ) {}
  icMoreVert = icMoreVert;
  ngOnInit(): void {
    //this.getApplications();
  }

  closeDrawer() {
    if (this.layoutService.isLtLg()) {
      this.drawer?.close();
    }
  }
}
