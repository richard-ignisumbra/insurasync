import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import {
  ChangeDetectorRef,
  Component,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from "@angular/core";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import { MatDrawer, MatDrawerMode } from "@angular/material/sidenav";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";

import { Application } from "../../../../core/api/v1/model/application";
import { ApplicationSection } from "../../../../core/api/v1/model/applicationSection";
import { ApplicationSectionsService } from "../../../../core/api/v1/api/applicationSections.service";
import { ApplicationService } from "../services/application-service.service";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { ConfigService } from "../../../../../@vex/config/config.service";
import { FormControl } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { IsLoadingService } from "@service-work/is-loading";
import { LayoutService } from "../../../../../@vex/services/layout.service";
import { MatDialog } from "@angular/material/dialog";
import { fadeInRight400ms } from "../../../../../@vex/animations/fade-in-right.animation";
import icEdit from "@iconify/icons-ic/twotone-edit";
import icMail from "@iconify/icons-ic/twotone-mail";
import icMenu from "@iconify/icons-ic/twotone-menu";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSearch from "@iconify/icons-ic/twotone-search";
import { map } from "rxjs/operators";
import { scaleIn400ms } from "../../../../../@vex/animations/scale-in.animation";

@Component({
  selector: "vex-view-application",
  templateUrl: "./view-application.component.html",
  styleUrls: ["./view-application.component.scss"],
  animations: [scaleIn400ms, fadeInRight400ms],
  encapsulation: ViewEncapsulation.None,
})
export class ViewApplicationComponent implements OnInit {
  isLoading: Observable<boolean>;
  private _applicationDetails = new BehaviorSubject<Application>(null);
  readonly ApplicationDetails$ = this._applicationDetails.asObservable();
  ColumnMode = ColumnMode;
  private parametersSubscription: any;
  private _applicationSections = new BehaviorSubject<ApplicationSection[]>(
    null
  );
  public panelMode: string = "over";

  readonly ApplicationSections$ = this._applicationSections.asObservable();

  isDesktop$ = this.layoutService.isDesktop$;
  isVerticalLayout$: Observable<boolean> = this.configService.select(
    (config) => config.layout === "vertical"
  );
  ltLg$ = this.layoutService.ltLg$;
  gtSm$ = this.layoutService.gtSm$;
  isOpen: boolean = false;
  drawerMode$: Observable<MatDrawerMode> = this.isDesktop$.pipe(
    map((isDesktop) => (isDesktop ? "side" : "over"))
  );

  searchCtrl = new FormControl();

  icMail = icMail;
  icSearch = icSearch;
  icEdit = icEdit;
  icMenu = icMenu;

  @ViewChild(MatDrawer, { static: true }) private drawer: MatDrawer;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,

    private applicationsService: ApplicationsService,
    private layoutService: LayoutService,
    private dialog: MatDialog,
    private readonly configService: ConfigService,
    private applicationSectionService: ApplicationSectionsService,
    public applicationService: ApplicationService,
    private changeDetectorRef: ChangeDetectorRef,
    private isLoadingService: IsLoadingService
  ) {}
  icMoreVert = icMoreVert;
  ngOnInit(): void {
    //this.getApplications();
    this.isLoading = this.isLoadingService.isLoading$({
      key: ["appSections", "memberDetails", "getApplicationSections"],
    });
    this.parametersSubscription = this.route.paramMap.subscribe(
      (params: ParamMap) => {
        let paramID = params.get("id");
        if (paramID) {
          let appId: number = Number(paramID);
          this.getApplicationDetails(appId);
          this.getApplicationSections(appId);
        }
      }
    );
    setTimeout(() => {
      console.log("hiding");
      this.panelMode = "side";
      this.isOpen = true;
      this.changeDetectorRef.detectChanges();
    }, 500);
  }
  getApplicationDetails(id: number) {
    console.log(id);
    this.isLoadingService.add({ key: "memberDetails" });
    this.applicationsService.apiApplicationsIdGet(id).subscribe((e) => {
      this._applicationDetails.next(e);
      console.log("successfully retrieved application details;");
      this.isLoadingService.remove({ key: "memberDetails" });
    });
  }
  getApplicationSections(id: number) {
    console.log("getApplicationSections");
    this.isLoadingService.add({ key: "appSections" });
    this.applicationSectionService
      .apiApplicationSectionsApplicationIdGet(id)
      .subscribe((e) => {
        console.log(e);

        this.applicationService.setSections(e);
        this.isLoadingService.remove({ key: "appSections" });
      });
  }
}
