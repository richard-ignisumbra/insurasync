import * as FileSaver from "file-saver";

import { BehaviorSubject, Observable, Subject } from "rxjs";
import { Component, OnInit } from "@angular/core";

import { ActivatedRoute } from "@angular/router";
import { AppUtilityService } from "../../../../../app/shared-utilities/app-utility.service";
import { ApplicationPreview } from "../../../../core/api/v1/model/applicationPreview";
import { ApplicationReportType } from "../../../../core/api/v1/model/applicationReportType";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { ApplicationsServiceV2 } from "../services/applications.service";
import { ColumnMode } from "@swimlane/ngx-datatable";
import { CreateApplicationComponent } from "../create-application/create-application.component";
import { IsLoadingService } from "@service-work/is-loading";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";

@Component({
  selector: "vex-application-list",
  templateUrl: "./application-list.component.html",
  styleUrls: ["./application-list.component.scss"],
})
export class ApplicationListComponent implements OnInit {
  private _applicationPreviews = new BehaviorSubject<ApplicationPreview[]>(
    null
  );
  readonly ApplicationPreviews$ = this._applicationPreviews.asObservable();
  private _applicationTypes = new BehaviorSubject<ApplicationReportType[]>([]);
  private _lineofCoverages = new BehaviorSubject<string[]>([]);

  ColumnMode = ColumnMode;
  constructor(
    private router: Router,
    private dialog: MatDialog,
    private _loadingService: IsLoadingService,
    private route: ActivatedRoute,
    private applicationsService: ApplicationsService,
    public appUtilityService: AppUtilityService,

    private ApplicationsServiceV2: ApplicationsServiceV2
  ) {}
  icMoreVert = icMoreVert;
  filterApplicationType?: number = null;
  readonly applicationTypes$ = this._applicationTypes.asObservable();
  readonly lineofCoverages$ = this._lineofCoverages.asObservable();
  selectedCoverageYear: string;
  filterStatus: string = "all";
  filterQuarter?: number = null;
  filterMonth?: number = null;
  filterLineofCoverage: string = "all";
  coverageyears: number[] = [];
  applications$: any;
  showQuarterlyFilter: boolean = false;
  showMonthlyFilter: boolean = false;
  ngOnInit(): void {
    this.buildCoverageyears();
    this.selectedCoverageYear = new Date().getFullYear().toString();
    this.getApplications();
    this.appUtilityService.UserProfile$.subscribe((data) => {});
    this.getApplicationTypes();
    this.getLineofCoverages();
  }
  getApplicationTypes() {
    console.log("new getApplicationTypes");
    this.ApplicationsServiceV2.getApplicationTypes().subscribe((types) => {
      this._applicationTypes.next(types);
    });
  }
  getLineofCoverages() {
    console.log("new Line of Coverages");
    this.ApplicationsServiceV2.getLineofCoverages().subscribe((lines) => {
      this._lineofCoverages.next(lines);
    });
  }
  buildCoverageyears() {
    let startYear: number = 2021;
    for (let i = startYear; i <= new Date().getFullYear() + 1; i++) {
      this.coverageyears.push(i);
    }
  }
  getApplications() {
    this._loadingService.add({ key: "appPreviews" });
    let filterappTypes = null;
    if (this.filterApplicationType) {
      console.log("application filter type - ", this.filterApplicationType);
      filterappTypes = this.filterApplicationType;
      this.applications$ = this.ApplicationsServiceV2.getFilteredApplications(
        Number(this.selectedCoverageYear),
        this.filterStatus,
        filterappTypes,
        this.filterQuarter,
        this.filterMonth,
        this.filterLineofCoverage
      );
      this.applications$.subscribe((applicationPreviews) => {
        this._applicationPreviews.next(applicationPreviews);
      });
    }
  }
  viewDetails(row: ApplicationPreview) {
    let id = row.applicationId;
    this.router.navigate(["applications/details", id, row.currentSectionId]);
  }
  onActivate(event: any) {
    if (event.type === "click" && event.row) {
      this.viewDetails(event.row);
    }
  }
  ExportData() {
    let applicationType = this._applicationTypes.value.filter((e) => {
      return e.applicationTypeId === this.filterApplicationType;
    });
    console.log("exportData", applicationType);
    if (applicationType.length > 0) {
      var appType = applicationType[0];

      this.ApplicationsServiceV2.exportApplications(
        Number(this.selectedCoverageYear),
        appType.reportType,
        this.filterStatus,
        this.filterQuarter,
        this.filterMonth,
        this.filterLineofCoverage,
        this.filterApplicationType
      ).subscribe((blob) => {
        console.log("got data!", blob.size);
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement("a");
        anchor.href = url;
        anchor.download = `applications-${Number(
          this.selectedCoverageYear
        )}.xlsx`;
        anchor.click();
        console.log("anchor", anchor);
        // For IE and Edge
        if (window.navigator && (window.navigator as any).msSaveOrOpenBlob) {
          (window.navigator as any).msSaveOrOpenBlob(
            blob,
            `applications-${Number(this.selectedCoverageYear)}.xlsx`
          );
        }

        window.URL.revokeObjectURL(url);
      });
    }
  }

  showCreateApplication() {
    let createApp = this.dialog.open(CreateApplicationComponent, {
      data: "banana",
      width: "600px",
    });
    createApp.afterClosed().subscribe((e) => {
      this.getApplications();
    });
  }
  goReports() {}
  public onCoverageYearChanged(event: any) {
    this.selectedCoverageYear = event.value;
    this.getApplications();
  }
  showhideQuarterlyFilter() {
    let applicationType = this._applicationTypes.value.filter((e) => {
      return e.applicationTypeId === this.filterApplicationType;
    });
    this.showQuarterlyFilter =
      applicationType.length == 1 &&
      applicationType[0].groupType === "Quarterly";
    if (!this.showQuarterlyFilter) {
      this.filterQuarter = null;
    } else {
      if (!this.filterQuarter) {
        this.filterQuarter = 0;
      }
    }
  }
  showhideMonthlyFilter() {
    let applicationType = this._applicationTypes.value.filter((e) => {
      return e.applicationTypeId === this.filterApplicationType;
    });
    this.showMonthlyFilter =
      applicationType.length == 1 && applicationType[0].groupType === "Monthly";
    if (!this.showMonthlyFilter) {
      this.filterMonth = null;
    } else {
      if (!this.filterMonth) {
        this.filterMonth = 0;
      }
    }
  }
  public onFiltersChanged(event: any) {
    this.showhideQuarterlyFilter();
    this.showhideMonthlyFilter();
    this.getApplications();
  }
}
