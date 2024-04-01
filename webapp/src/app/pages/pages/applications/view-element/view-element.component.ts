import { ActivatedRoute, ParamMap, Router } from "@angular/router";
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
import { FormGroupDirective, NgForm } from "@angular/forms";
import { ErrorStateMatcher } from "@angular/material/core";

/** Error when invalid control is dirty, touched, or submitted. */
export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(
    control: FormControl | null,
    form: FormGroupDirective | NgForm | null
  ): boolean {
    const isSubmitted = form && form.submitted;
    return !!(
      control &&
      control.invalid &&
      (control.dirty || control.touched || isSubmitted)
    );
  }
}
@Component({
  selector: "[formGroup] view-element, [formGroupName] view-element",
  templateUrl: "./view-element.component.html",
  styleUrls: ["./view-element.component.scss"],
  animations: [scaleIn400ms, fadeInRight400ms],
  encapsulation: ViewEncapsulation.None,
})
export class ViewElementComponent
  implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy
{
  private _sectionId: number;
  private _rowId: number = -1;
  term$ = new Subject<string>();
  private textboxChange: Subscription;

  get RowId() {
    return this._rowId;
  }

  @Input() set RowId(rowId: number) {
    this._rowId = rowId;
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
  }
  get ApplicationElement() {
    return this._applicationElement;
  }
  @Input() showLabel: boolean = true;
  private _applicationId: number;
  @Input() set ApplicationId(applicationId: number) {
    this._applicationId = applicationId;
  }

  get ApplicationId() {
    return this._applicationId;
  }
  ElementTypeEnum = ElementType;
  isDesktop$ = this.layoutService.isDesktop$;
  ltLg$ = this.layoutService.ltLg$;
  savedResponseValue: any;
  runonce: boolean = false;
  public form: FormGroup;
  elementType = ElementType;
  matcher = new MyErrorStateMatcher();
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private applicationsService: ApplicationsService,
    private layoutService: LayoutService,
    private controlContainer: ControlContainer,
    private readonly changeDetectorRef: ChangeDetectorRef,
    private applicationSectionElementsService: ApplicationSectionElementsService
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
    let validators = [];
    if (this.ApplicationElement.isRequired) {
      validators.push(Validators.required);
    }

    let response = this.ApplicationElement.responses.find(
      (e) => e.rowId === this.RowId
    );

    let initialValue;
    if (response != undefined) {
      initialValue = this.GetElementResponseValue(response);
    } else {
      initialValue = "";
    }

    this.savedResponseValue = initialValue;
    let ctrl = new FormControl(initialValue);
    switch (this.ApplicationElement.elementType) {
      case ElementType.Email:
      case ElementType.Currency:
      case ElementType.Date:
      case ElementType.Integer:
      case ElementType.LargeText:
      case ElementType.Text:
        let maxLength = this.ApplicationElement.maxLength
          ? this.ApplicationElement.maxLength
          : 50;

        validators.push(Validators.maxLength(maxLength));

        break;
    }

    switch (this.ApplicationElement.elementType) {
      case ElementType.Email:
        validators.push(Validators.email);
        break;
      case this.elementType.MultiSelect:
      case this.elementType.SingleSelect:
        break;
    }
    if (this.ApplicationElement.elementType !== ElementType.Html) {
      this.form.addControl(this.formControlName(), ctrl);
      Promise.resolve().then(() => {
        ctrl.clearValidators();
        ctrl.setValidators(validators);
        ctrl.updateValueAndValidity();
        this.ApplicationElement.readonly ? ctrl.disable() : ctrl.enable();
      });
      if (this.ApplicationElement.elementType === ElementType.Date) {
        ctrl.valueChanges
          .pipe(debounceTime(1000), distinctUntilChanged())
          .subscribe((res) => {
            console.log("this is the date object after a change");
            this.SaveElement(res);
          });
      } else {
        ctrl.valueChanges
          .pipe(debounceTime(1000), distinctUntilChanged())
          .subscribe((res) => {
            console.log("an object saving that is not a date");
            this.SaveElement(res);
          });
      }
    }

    //  this.changeDetectorRef.detectChanges();
  }

  ngOnDestroy() {
    //remember to unsubscribe on destroy

    if (this.textboxChange) {
      this.textboxChange.unsubscribe();
      this.textboxChange = null;
    }
  }
  SaveElement(event: any) {
    console.log("element - " + this.RowId);
    if (this.ApplicationElement.elementType === this.elementType.Html) {
      return false;
    }
    const response = this.GetElementResponse(this.ApplicationElement);
    console.log(
      "the response from a change",
      response,
      this.savedResponseValue,
      this.GetElementResponseValue(response)
    );
    if (
      _.isEqual(
        this.GetElementResponseValue(response),
        this.savedResponseValue
      ) === false
    ) {
      console.log(
        "trying to save",
        this.GetElementResponseValue(response),
        this.savedResponseValue
      );
      this.applicationSectionElementsService
        .apiApplicationSectionElementsApplicationIdSectionSectionIdElementIdPost(
          this.ApplicationId,
          this.SectionId,
          this.ApplicationElement.elementId,
          response
        )
        .subscribe((result) => {
          console.log("raising event", response);
          this.Changed.emit(response);
        });
    }
  }

  GetElementResponseValue(response: ApplicationElementResponse) {
    if (!response) {
      return null;
    }

    switch (this.ApplicationElement.elementType) {
      case this.elementType.Attachment:
        break;
      case this.elementType.Checkbox:
        if (response.longTextResponse && response.longTextResponse !== "") {
          let parsedResponse;

          if (response.longTextResponse !== null) {
            // Check if the string doesn't start with [
            if (!response.longTextResponse.startsWith("[")) {
              // Surround it with [ and ]
              response.longTextResponse = `[${response.longTextResponse}]`;
            }

            try {
              parsedResponse = JSON.parse(response.longTextResponse);
            } catch (error) {
              console.error("Failed to parse longTextResponse:", error);
            }
          }

          return parsedResponse;
        } else {
          return false;
        }

      case this.elementType.Currency:
        return response.currencyResponse;

      case this.elementType.Date:
        if (response.dateResponse != null && response.dateResponse != "") {
          console.log("this is not a date", response);
          const theDate = new Date(response.dateResponse);
          const yyyy = theDate.getFullYear();
          let mm = theDate.getMonth() + 1; // Months start at 0!
          let dd = theDate.getDate();
          let sdd = dd.toString();
          let smm = mm.toString();
          if (dd < 10) sdd = "0" + dd;
          if (mm < 10) smm = "0" + mm;

          const formattedDate = `${yyyy}-${smm}-${sdd}`;
          console.log("this is the date - " + formattedDate);
          if (formattedDate != "1969-12-31") {
            return formattedDate;
          }
        }

        break;
      case this.elementType.Email:
        return response.textResponse;

      case this.elementType.Html:
        break;
      case this.elementType.Integer:
        return response.intResponse;

      case this.elementType.LargeText:
        return response.longTextResponse;

      case this.elementType.MultiSelect:
        let parsedResponse;

        if (response.longTextResponse !== null) {
          // Check if the string doesn't start with [
          if (!response.longTextResponse.startsWith("[")) {
            // Surround it with [ and ]
            response.longTextResponse = `[${response.longTextResponse}]`;
          }

          try {
            parsedResponse = JSON.parse(response.longTextResponse);
          } catch (error) {
            console.error("Failed to parse longTextResponse:", error);
          }
        }

        return parsedResponse;

      case this.elementType.SingleSelect:
        if (response.longTextResponse) return response.longTextResponse;

        break;

      case this.elementType.Table:
        break;
      case this.elementType.Text:
        return response.textResponse;
    }
  }
  public HasPreviousResponse(): boolean {
    if (this.ApplicationElement.previousResponses == null) {
      return false;
    }
    var previousResponse = this.ApplicationElement.previousResponses.find(
      (e) => e.rowId === this.RowId
    );
    if (previousResponse) {
      var value = this.GetElementResponseValue(previousResponse);
      if (value != null && value != "") return true;
    } else {
      return false;
    }
  }
  public PreviousResponse(): string {
    var previousResponse = this.ApplicationElement.previousResponses.find(
      (e) => e.rowId === this.RowId
    );

    return this.GetElementResponseValue(previousResponse);
  }
  GetElementResponse(element: ApplicationElement): ApplicationElementResponse {
    let response: ApplicationElementResponse = {
      applicationId: this.ApplicationId,
      elementId: element.elementId,
      rowId: this.RowId,
    };
    let formValue;
    let formControlName = this.formControlName();
    console.log("getting element response", formControlName);
    if (this.form.get(formControlName)) {
      formValue = this.form.get(formControlName).value;
    } else {
      formValue = "";
    }

    switch (element.elementType) {
      case this.elementType.Attachment:
        break;
      case this.elementType.Checkbox:
        response.longTextResponse = JSON.stringify(formValue);
        break;
      case this.elementType.Currency:
        response.currencyResponse = formValue;
        break;
      case this.elementType.Date:
        response.dateResponse = formValue;
        break;
      case this.elementType.Email:
        response.textResponse = formValue;
        break;
      case this.elementType.Html:
        response.textResponse = formValue;
        break;
      case this.elementType.Integer:
        response.intResponse = formValue;
        break;
      case this.elementType.LargeText:
        response.longTextResponse = formValue;
        break;
      case this.elementType.MultiSelect:
        response.longTextResponse = JSON.stringify(formValue);
        break;
      case this.elementType.SingleSelect:
        response.longTextResponse = formValue;
        break;

      case this.elementType.Table:
        break;
      case this.elementType.Text:
        response.textResponse = formValue;
        break;
    }
    console.log("built - ", response);
    return response;
  }
  @Output() Changed = new EventEmitter<any>();
  public formControlName() {
    const elementId = this.ApplicationElement.elementId;
    const shortName = this.ApplicationElement.shortName.replace(
      /[^a-zA-Z0-9]/g,
      "_"
    );
    const rowId = this.RowId;

    return `${elementId}-${shortName}-${rowId}`;
  }

  GetElementWidth(): number | null {
    if (this.ApplicationElement.width) {
      return this.ApplicationElement.width;
    } else {
      return null;
    }
  }
  GetHTMLClasses(): string {
    let classes = "mt-0 mb-4 text-secondary";
    if (this.GetElementWidth()) {
      classes += " w-" + this.GetElementWidth() + " ";
    }
    return classes;
  }
  GetIndentSpaces(): string {
    if (this.ApplicationElement.indentSpaces) {
      return `padding-left: ${this.ApplicationElement.indentSpaces}em;`;
    }
  }
}
