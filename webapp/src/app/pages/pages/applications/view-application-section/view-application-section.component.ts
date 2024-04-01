import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import {
  Component,
  HostListener,
  Input,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from "@angular/core";
import { FormBuilder, ValidationErrors, Validators } from "@angular/forms";
import { MatDrawer, MatDrawerMode } from "@angular/material/sidenav";
import { MsalBroadcastService, MsalService } from "@azure/msal-angular";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { debounceTime, map, switchMap, takeUntil } from "rxjs/operators";

import { Application } from "../../../../core/api/v1/model/application";
import { ApplicationElement } from "../../../../core/api/v1/model/applicationElement";
import { ApplicationElementResponse } from "../../../../core/api/v1/model/applicationElementResponse";
import { ApplicationSection } from "../../../../core/api/v1/model/applicationSection";
import { ApplicationSectionElementsService } from "../../../../core/api/v1/api/applicationSectionElements.service";
import { ApplicationSectionsService } from "../../../../core/api/v1/api/applicationSections.service";
import { ApplicationService } from "../services/application-service.service";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { ElementType } from "src/app/core/api/v1";
import { FormControl } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { IsLoadingService } from "@service-work/is-loading";
import { LayoutService } from "../../../../../@vex/services/layout.service";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ScrollbarComponent } from "../../../../../@vex/components/scrollbar/scrollbar.component";
import { SubmitApplicationComponent } from "../submit-application/submit-application.component";
import { ViewportScroller } from "@angular/common";
import { fadeInRight400ms } from "../../../../../@vex/animations/fade-in-right.animation";
import { fadeInUp400ms } from "src/@vex/animations/fade-in-up.animation";
import icEdit from "@iconify/icons-ic/twotone-edit";
import icMail from "@iconify/icons-ic/twotone-mail";
import icMenu from "@iconify/icons-ic/twotone-menu";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSearch from "@iconify/icons-ic/twotone-search";
import { protectedResources } from "../../../../auth-config";
import { scaleIn400ms } from "../../../../../@vex/animations/scale-in.animation";
import { stagger } from "@angular/animations";
import { stagger20ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-view-application-section",
  templateUrl: "./view-application-section.component.html",
  styleUrls: ["./view-application-section.component.scss"],
  animations: [scaleIn400ms, fadeInRight400ms, fadeInUp400ms],
  encapsulation: ViewEncapsulation.None,
})
export class ViewApplicationSectionComponent implements OnInit {
  @ViewChild("vexScrollbar") vexScrollbar: ScrollbarComponent;

  ColumnMode = ColumnMode;

  private parametersSubscription: any;
  public isFirst: boolean = false;
  public isLast: boolean = false;
  elementType = ElementType;
  form = this.fb.group({});
  availableHeight: number = 0;
  offsetHeight: number = 250;
  pageYoffset: number;
  applicationSections: ApplicationSection[];
  @HostListener("window:scroll", ["$event"]) onScroll(event) {
    this.pageYoffset = window.pageYOffset;
  }
  private _applicationElements = new BehaviorSubject<ApplicationElement[]>(
    null
  );

  readonly applicationElements$ = this._applicationElements.asObservable();
  private _isReadonly = new BehaviorSubject<boolean>(false);
  readonly isReadonly$ = this._isReadonly.asObservable();
  isDesktop$ = this.layoutService.isDesktop$;
  ltLg$ = this.layoutService.ltLg$;
  drawerMode$: Observable<MatDrawerMode> = this.isDesktop$.pipe(
    map((isDesktop) => (isDesktop ? "side" : "over"))
  );
  drawerOpen = true;

  searchCtrl = new FormControl();
  public PreviousSectionId: number;
  public NextSectionId: number;
  icMail = icMail;
  icSearch = icSearch;
  icEdit = icEdit;
  icMenu = icMenu;
  applicationId?: number;
  sectionId?: number;
  private unsubscribe = new Subject<void>();
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private applicationsService: ApplicationsService,
    private layoutService: LayoutService,
    private dialog: MatDialog,
    private applicationSectionService: ApplicationSectionsService,
    private applicationSectionElementsService: ApplicationSectionElementsService,
    private fb: FormBuilder,
    private applicationService: ApplicationService,
    private _snackBar: MatSnackBar,
    private scroll: ViewportScroller,
    private isLoadingService: IsLoadingService
  ) {}
  icMoreVert = icMoreVert;

  ngOnInit(): void {
    //this.getApplications();
    this.calculateSize();

    this.parametersSubscription = this.route.paramMap.subscribe(
      (params: ParamMap) => {
        let paramID = params.get("id");
        if (paramID) {
          this.applicationId = Number(paramID);
          console.log(`application Id - ${this.applicationId}`);
        }
        let sectionId = params.get("sectionId");
        if (sectionId) {
          this.sectionId = Number(sectionId);
          console.log(`section Id - ${this.sectionId}`);
        }
        if (this.applicationId && this.sectionId) {
          this.applicationService.Sections$.subscribe((subscriptions) => {
            this.applicationSections = subscriptions;
            if (subscriptions && subscriptions.length > 0) {
              const firstSection = subscriptions[0];
              const lastSection = subscriptions[subscriptions.length - 1];
              const index = this.applicationSections.findIndex(
                (x) => x.applicationSectionId === this.sectionId
              );
              this.isFirst =
                firstSection.applicationSectionId === this.sectionId;
              if (firstSection.applicationSectionId !== this.sectionId) {
                this.PreviousSectionId =
                  this.applicationSections[index - 1].applicationSectionId;
              }
              this.isLast = lastSection.applicationSectionId === this.sectionId;
              if (lastSection.applicationSectionId !== this.sectionId) {
                this.NextSectionId =
                  this.applicationSections[index + 1].applicationSectionId;
              }
            }
          });
          this.scrollToTop();
          this.applicationService.setActiveSection(this.sectionId);

          this.resetForm();
          this.getApplicationSectionElements(
            this.applicationId,
            this.sectionId
          );
        } else {
        }
      }
    );
  }
  scrollToTop() {
    this.scroll.scrollToPosition([0, 0]);
    if (this.vexScrollbar && this.vexScrollbar.scrollbarRef)
      this.vexScrollbar.scrollbarRef.getScrollElement().scrollTop = 0;
  }
  calculateSize() {
    this.availableHeight = window.innerHeight - this.offsetHeight;
  }
  @HostListener("window:resize", ["$event"])
  onResize(event) {
    this.calculateSize();
  }
  resetForm() {
    Object.keys(this.form.controls).forEach((key) => {
      this.form.removeControl(key);
    });
  }
  goNext(readonly: boolean) {
    if (readonly == false) {
      if (this.isSectionValid()) {
        console.log("go next ", this.NextSectionId);

        this.applicationService.CompleteSection(
          this.sectionId,
          this.applicationId,
          true
        );

        this.router.navigateByUrl(
          `/applications/details/${this.applicationId}/${this.NextSectionId}`
        );
      }
    } else {
      this.router.navigateByUrl(
        `/applications/details/${this.applicationId}/${this.NextSectionId}`
      );
    }
  }
  goPrevious() {
    console.log("go previous ", this.PreviousSectionId);
    this.router.navigateByUrl(
      `/applications/details/${this.applicationId}/${this.PreviousSectionId}`
    );
  }

  showSubmitApplication() {
    if (this.isSectionValid() && this.isApplicationValid()) {
      let theDialog = this.dialog.open(SubmitApplicationComponent, {
        data: this.applicationId,
        width: "600px",
      });
    } else {
    }
  }
  isApplicationValid(): boolean {
    let isApplicationValid = this.applicationService.allSectionsCompleted();
    if (isApplicationValid === false) {
      this.openSnackBar("Please complete all sections", "Close", 15000);
    }
    return isApplicationValid;
  }
  isSectionValid(): boolean {
    let isApplicationValid = false;
    var elements = this._applicationElements.getValue();
    var isAttachmentsValid = true;
    if (this.form.valid) {
      elements.forEach((element) => {
        if (element.elementType === ElementType.Attachment) {
          if (element.isRequired && element.responses.length === 0) {
            isAttachmentsValid = false;
          }
        }
      });

      if (!this.form.valid || !isAttachmentsValid) {
        let errorMessage = "Please complete all required fields";
        if (!isAttachmentsValid) {
          errorMessage += " and upload all required attachments";
        }
        this.openSnackBar(errorMessage, "Close", 15000);
        console.log(
          "%c ==>> Validation Errors: ",
          "color: red; font-weight: bold; font-size:25px;"
        );

        let totalErrors = 0;

        Object.keys(this.form.controls).forEach((key) => {
          const controlErrors: ValidationErrors = this.form.get(key).errors;
          if (controlErrors != null) {
            this.form.controls[key].markAsTouched();
            totalErrors++;
            Object.keys(controlErrors).forEach((keyError) => {
              console.log(
                "Key control: " +
                  key +
                  ", keyError: " +
                  keyError +
                  ", err value: ",
                controlErrors[keyError]
              );
            });
          }
        });
        this.applicationService.CompleteSection(
          this.sectionId,
          this.applicationId,
          false
        );
        console.log("Number of errors: ", totalErrors);
      } else {
        isApplicationValid = true;
        this.applicationService.CompleteSection(
          this.sectionId,
          this.applicationId,
          true
        );
      }

      return isApplicationValid;
    }
  }
  public openSnackBar(
    message: string,
    action: string,
    duration: number = 3000
  ) {
    this._snackBar.open(message, action, { duration: duration });
  }
  getApplicationSectionElements(applicationId: number, sectionId: number) {
    console.log("getApplicationSections");
    this.isLoadingService.add({ key: "appElements" });
    this.applicationSectionElementsService
      .apiApplicationSectionElementsApplicationIdSectionSectionIdGet(
        applicationId,
        sectionId
      )
      .subscribe((e) => {
        console.log(e);
        this._applicationElements.next(e);
        if (e.length > 0) {
          var firstelement = e[0];
          this._isReadonly.next(firstelement.readonly);
        }

        this.isLoadingService.remove({ key: "appElements" });
      });
  }
}
