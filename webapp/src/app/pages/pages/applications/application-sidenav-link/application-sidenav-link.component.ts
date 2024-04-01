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

import { Application } from "../../../../core/api/v1/model/application";
import { ApplicationSection } from "../../../../core/api/v1/model/applicationSection";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { HttpClient } from "@angular/common/http";
import icFactCheck from "@iconify/icons-ic/twotone-check-circle";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import { protectedResources } from "../../../../auth-config";

@Component({
  selector: "application-sidenav-link",
  templateUrl: "./application-sidenav-link.component.html",
  styleUrls: ["./application-sidenav-link.component.scss"],
})
export class ApplicationSidenavLinkComponent implements OnInit {
  applicationsEndpoint: string = protectedResources.applicationsApi.endpoint;
  applicationDetails: Application;
  ColumnMode = ColumnMode;

  @Input() appSection: ApplicationSection;
  private parametersSubscription: any;

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private applicationsService: ApplicationsService
  ) {}
  icMoreVert = icMoreVert;
  icFactCheck = icFactCheck;
  ngOnInit(): void {
    //this.getApplications();
  }
  getApplicationDetails(id: number) {}
  linkRoute(): string {
    let URL = "";
    URL = `/applications/details/${this.appSection.applicationSectionId}`;
    console.log("prop - linkRoute");
    console.log(URL);
    return URL;
  }
  isActive(): boolean {
    return true;
  }
}
