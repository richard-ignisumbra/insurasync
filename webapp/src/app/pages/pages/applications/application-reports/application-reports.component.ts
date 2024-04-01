import * as FileSaver from "file-saver";

import { BehaviorSubject, Observable, Subject } from "rxjs";
import { Component, OnDestroy, OnInit } from "@angular/core";

import { ActivatedRoute } from "@angular/router";
import { AppUtilityService } from "../../../../shared-utilities/app-utility.service";
import { ApplicationPreview } from "../../../../core/api/v1/model/applicationPreview";
import { ApplicationReport } from "../../../../core/api/v1/model/applicationReport";
import { ApplicationReportType } from "../../../../core/api/v1/model/applicationReportType";
import { ApplicationTypesService } from "../../../../core/api/v1/api/applicationTypes.service";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { ApplicationsServiceV2 } from "../services/applications.service";
import { ColumnMode } from "@swimlane/ngx-datatable";
import { CreateApplicationComponent } from "../create-application/create-application.component";
import { IsLoadingService } from "@service-work/is-loading";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icinsert_chart from "@iconify/icons-ic/twotone-insert-chart";
import { takeUntil } from "rxjs/operators";

@Component({
  selector: "vex-application-reports",
  templateUrl: "./application-reports.component.html",
  styleUrls: ["./application-reports.component.scss"],
  animations: [fadeInUp400ms],
})
export class ApplicationReportsComponent implements OnInit, OnDestroy {
  private unsubscribe$ = new Subject<void>();

  private _applicationPreviews = new BehaviorSubject<ApplicationPreview[]>([]);

  readonly ApplicationPreviews$ = this._applicationPreviews.asObservable();
  private _applicationReports = new BehaviorSubject<ApplicationReport[]>([]);
  private _reportTypes = new BehaviorSubject<ApplicationReportType[]>([]);

  readonly ApplicationReports$ = this._applicationReports.asObservable();
  ColumnMode = ColumnMode;
  constructor(
    private router: Router,
    private dialog: MatDialog,
    private _loadingService: IsLoadingService,
    private route: ActivatedRoute,
    private applicationsService: ApplicationsService,
    public appUtilityService: AppUtilityService,
    private applicationTypesService: ApplicationTypesService,
    private applicationsServiceV2: ApplicationsServiceV2
  ) {}
  icMoreVert = icMoreVert;
  icinsert_chart = icinsert_chart;
  selectedCoverageYear: string;
  filterStatus: string = "all";
  filterReportType: string = "1";
  coverageyears: number[] = [];

  readonly reportTypes$ = this._reportTypes.asObservable();

  applications$: Observable<ApplicationPreview[]>;

  ngOnInit(): void {
    this.initializeData();
  }

  initializeData() {
    this.buildCoverageyears();
    this.selectedCoverageYear = new Date().getFullYear().toString();
    this.loadData();
  }
  loadData() {
    this.appUtilityService.UserProfile$.pipe(
      takeUntil(this.unsubscribe$)
    ).subscribe(() => {});
    this.applicationsServiceV2
      .getApplicationReportTypes()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((data) => {
        console.log("applicationReportTypes", data);
        this._reportTypes.next(data);
      });

    this.getApplications();
  }
  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  buildCoverageyears() {
    let startYear: number = 2021;
    for (let i = startYear; i <= new Date().getFullYear() + 1; i++) {
      this.coverageyears.push(i);
    }
  }
  getApplications() {
    this._loadingService.add({ key: "appPreviews" });
    let reportType: number | null = Number(this.filterReportType);
    this.applications$ = this.applicationsService
      .apiApplicationsGet(
        Number(this.selectedCoverageYear),
        this.filterStatus,
        reportType
      )
      .pipe(takeUntil(this.unsubscribe$));
    this.applications$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((applicationPreviews) => {
        this._applicationPreviews.next(applicationPreviews);
      });
    this.applicationTypesService
      .apiApplicationTypesReportsGet(reportType)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((reports) => {
        this._applicationReports.next(reports);
      });
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
  runReport(reportId: number) {
    this.ExportData(reportId);
  }

  ExportData(reportId: number) {
    this.applicationsServiceV2
      .ExportApplicationReport(
        Number(this.selectedCoverageYear),
        reportId,
        this.filterStatus,

        null,
        null
      )
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((data) => {
        console.log(data);

        const blob = new Blob([data], { type: "application/vnd.ms-excel" });

        const fileName: string = `applicationExport-${this.selectedCoverageYear}.xlsx`;
        FileSaver.saveAs(blob, fileName);
      });
  }
  onTabChange(event: any) {
    this.getApplications();
  }
  showCreateApplication() {
    let createApp = this.dialog.open(CreateApplicationComponent, {
      data: "banana",
      width: "600px",
    });
    createApp
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((e) => {
        this.getApplications();
      });
  }
  public onCoverageYearChanged(event: any) {
    this.selectedCoverageYear = event.value;
    this.getApplications();
  }
  public onFiltersChanged(event: any) {
    this.getApplications();
  }
}
