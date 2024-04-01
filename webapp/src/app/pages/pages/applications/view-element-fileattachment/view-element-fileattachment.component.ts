import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import { ApplicationFileUploadService } from "../../../../core/api/v1/api/applicationFileUpload.service";
import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  AfterViewInit,
  Output,
  ChangeDetectorRef,
  ViewChild,
  AfterViewChecked,
  ViewEncapsulation,
  OnDestroy,
} from "@angular/core";
import { ApplicationElementResponse } from "../../../../core/api/v1/model/applicationElementResponse";

import { MatDrawer, MatDrawerMode } from "@angular/material/sidenav";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import {
  FormBuilder,
  FormGroup,
  Validators,
  ControlContainer,
} from "@angular/forms";

import { Application } from "../../../../core/api/v1/model/application";
import { ApplicationElement } from "../../../../core/api/v1/model/applicationElement";
import { ApplicationSection } from "../../../../core/api/v1/model/applicationSection";
import { ApplicationSectionElementsService } from "../../../../core/api/v1/api/applicationSectionElements.service";
import { ApplicationSectionsService } from "../../../../core/api/v1/api/applicationSections.service";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { ElementType } from "../../../../core/api/v1/model/elementType";
import { FormControl } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { LayoutService } from "../../../../../@vex/services/layout.service";
import { MatDialog } from "@angular/material/dialog";
import { Observable, Subject, EMPTY, Subscription, timer } from "rxjs";
import { fadeInRight400ms } from "../../../../../@vex/animations/fade-in-right.animation";
import icEdit from "@iconify/icons-ic/twotone-edit";
import icMail from "@iconify/icons-ic/twotone-mail";
import icMenu from "@iconify/icons-ic/twotone-menu";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSearch from "@iconify/icons-ic/twotone-search";
import _ from "lodash";
import {
  map,
  debounceTime,
  distinctUntilChanged,
  switchMap,
  debounce,
} from "rxjs/operators";
import { protectedResources } from "../../../../auth-config";
import { scaleIn400ms } from "../../../../../@vex/animations/scale-in.animation";
import { ElementSaveResponse } from "src/app/core/api/v1";

interface fileUpload {
  FileId: string;
  FileName: string;
}
@Component({
  selector:
    "[formGroup] view-element-fileattachment, [formGroupName] view-element-fileattachment",
  templateUrl: "./view-element-fileattachment.component.html",
  styleUrls: ["./view-element-fileattachment.component.scss"],
  animations: [scaleIn400ms, fadeInRight400ms],
  encapsulation: ViewEncapsulation.None,
})
export class ViewElementFileAttachmentComponent
  implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy
{
  private _sectionId: number;
  private _rowId: number = -1;
  term$ = new Subject<string>();
  private textboxChange: Subscription;
  public form: FormGroup;
  public file_upload_config = {
    API: "api/ApplicationFileUpload",

    is_multiple_selection_allowed: true,
    data: null,
    elementId: 0,
    rowId: 0,
    applicationId: 0,
  };
  get RowId() {
    return this._rowId;
  }

  @Input() set RowId(rowId: number) {
    this._rowId = rowId;
    this.file_upload_config.rowId = rowId;
  }
  get SectionId() {
    return this._sectionId;
  }
  @Input() set SectionId(sectionId: number) {
    this._sectionId = sectionId;
  }
  private _applicationElement: ApplicationElement;
  @Input() set ApplicationElement(applicationElement: ApplicationElement) {
    this._applicationElement = applicationElement;
    this.file_upload_config.elementId = applicationElement.elementId;
  }
  get ApplicationElement() {
    return this._applicationElement;
  }
  @Input() showLabel: boolean = true;
  private _applicationId: number;
  @Input() set ApplicationId(applicationId: number) {
    this._applicationId = applicationId;
    this.file_upload_config.applicationId = applicationId;
  }

  get ApplicationId() {
    return this._applicationId;
  }
  ElementTypeEnum = ElementType;
  isDesktop$ = this.layoutService.isDesktop$;
  ltLg$ = this.layoutService.ltLg$;
  savedResponseValue: any;
  runonce: boolean = false;

  elementType = ElementType;
  @Input()
  requiredFileType: string;

  fileName = "";
  uploadProgress: number;
  uploadSub: Subscription;

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private applicationsService: ApplicationsService,
    private layoutService: LayoutService,
    private controlContainer: ControlContainer,
    private readonly changeDetectorRef: ChangeDetectorRef,
    private applicationSectionElementsService: ApplicationSectionElementsService,
    private applicationFileUploadService: ApplicationFileUploadService
  ) {}
  icMoreVert = icMoreVert;
  ngOnInit(): void {
    //this.getApplications();
    this.form = <FormGroup>this.controlContainer.control;
    if (this.ApplicationElement && this.runonce === false) {
      this.InitElement();
      this.runonce = true;
    }
  }
  ngAfterViewInit() {}
  ngAfterViewChecked(): void {}
  private InitElement() {
    console.log("initElement", this.ApplicationElement);

    let validators = [];
    if (this.ApplicationElement.isRequired) {
      validators.push(Validators.required);
    }

    let response = this.ApplicationElement.responses.find(
      (e) => e.rowId === this.RowId
    );
  }

  ngOnDestroy() {
    //remember to unsubscribe on destroy

    if (this.textboxChange) {
      this.textboxChange.unsubscribe();
      this.textboxChange = null;
    }
  }
  UploadSuccess(uploadResults: string) {
    console.log("UploadSuccess", uploadResults);
    const response =
      this.ApplicationElement.responses &&
      this.ApplicationElement.responses.length === 1
        ? this.ApplicationElement.responses[0]
        : null;
    response.longTextResponse = uploadResults;
    this.changeDetectorRef.detectChanges();
  }

  GetElementResponseValue(): fileUpload[] {
    const response =
      this.ApplicationElement.responses &&
      this.ApplicationElement.responses.length === 1
        ? this.ApplicationElement.responses[0]
        : null;
    if (!response) {
      return null;
    }
    const results =
      response.longTextResponse !== ""
        ? JSON.parse(response.longTextResponse)
        : [];
    return results;
  }
  public DownloadFile(fileId: string) {
    console.log(`downloading ${fileId}`);
    this.applicationFileUploadService
      .apiApplicationFileUploadApplicationIdElementIdRowIdDownloadGet(
        this.ApplicationId,
        this.ApplicationElement.elementId,
        this.RowId,
        fileId
      )
      .subscribe((data) => {
        console.log("sas URL", data);
        (<any>window).saveAs(data, fileId);
      });
  }
  @Output() Changed = new EventEmitter<any>();
  public formControlName() {
    return `${this.ApplicationElement.elementId}-${this.ApplicationElement.shortName}-${this.RowId}`;
  }
  GetElementWidth(): number | null {
    if (this.ApplicationElement.width) {
      return this.ApplicationElement.width;
    } else {
      return null;
    }
  }
}
