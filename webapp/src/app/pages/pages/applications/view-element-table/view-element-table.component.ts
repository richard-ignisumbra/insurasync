import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import {
  ChangeDetectorRef,
  Component,
  Input,
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
import { FormBuilder, Validators } from "@angular/forms";
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
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { CurrencyPipe } from "@angular/common";
import { ElementType } from "src/app/core/api/v1";
import { FormControl } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { LayoutService } from "../../../../../@vex/services/layout.service";
import { cloneDeep } from "lodash";
import { fadeInRight400ms } from "../../../../../@vex/animations/fade-in-right.animation";
import icEdit from "@iconify/icons-ic/twotone-edit";
import icMail from "@iconify/icons-ic/twotone-mail";
import icMenu from "@iconify/icons-ic/twotone-menu";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSearch from "@iconify/icons-ic/twotone-search";
import { protectedResources } from "../../../../auth-config";
import { scaleIn400ms } from "../../../../../@vex/animations/scale-in.animation";

interface rowTemplate {
  rowId: number;
  elements: ApplicationElement[];
}
interface columnTemplate {
  elementId: number;
  label: string;
  width?: number;
  showFooter: boolean;
  footerValue: string;
  elementType: string;
}
interface TableData {
  columns: columnTemplate[];
  initialRows: ApplicationElement[];
}
@Component({
  selector: "vex-view-element-table",
  templateUrl: "./view-element-table.component.html",
  styleUrls: ["./view-element-table.component.scss"],
  animations: [scaleIn400ms, fadeInRight400ms],
  encapsulation: ViewEncapsulation.None,
})
export class ViewElementTableComponent implements OnInit {
  applicationDetails: Application;
  ColumnMode = ColumnMode;
  public data: any[] = [];

  public columns: columnTemplate[] = [];
  public initialRows: ApplicationElement[] = [];

  private _tableData = new BehaviorSubject<TableData>(null);
  readonly TableData$ = this._tableData.asObservable();

  private _applicationId: number;
  @Input() set ApplicationId(applicationId: number) {
    console.log("set table applicationid", applicationId);
    this._applicationId = applicationId;
    this.InitTable();
  }

  private createInitialRows() {
    this.initialRows = this.ApplicationElement.selectOptions.map((e) => {
      return {
        elementType: ElementType.Html,
        longText: e.label,
        responses: [],
        elementId: 0,
        width: 52,
      };
    });
    if (this.initialRows.length > 0) {
      let nonEmptyValues = this.ApplicationElement.selectOptions.filter(
        (e) => e.label !== ""
      );
      if (nonEmptyValues.length > 0)
        this.columns.push({
          elementId: 0,
          label: "",
          showFooter: false,
          footerValue: "0",
          elementType: ElementType.Html,
        });
    }
  }
  private _applicationElement: ApplicationElement;
  @Input() set ApplicationElement(applicationElement: ApplicationElement) {
    this._applicationElement = applicationElement;
    console.log("set table applicationelement", applicationElement);
    this.InitTable();
  }
  get ApplicationElement() {
    return this._applicationElement;
  }

  get ApplicationId() {
    return this._applicationId;
  }
  elementType = ElementType;
  form = this.fb.group({});
  applicationSections: ApplicationSection[];
  applicationElements: ApplicationElement[];
  isDesktop$ = this.layoutService.isDesktop$;
  ltLg$ = this.layoutService.ltLg$;
  drawerMode$: Observable<MatDrawerMode> = this.isDesktop$.pipe(
    map((isDesktop) => (isDesktop ? "side" : "over"))
  );
  drawerOpen = true;

  searchCtrl = new FormControl();

  icMail = icMail;
  icSearch = icSearch;
  icEdit = icEdit;
  icMenu = icMenu;
  private unsubscribe = new Subject<void>();
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private applicationsService: ApplicationsService,
    private layoutService: LayoutService,
    private readonly changeDetectorRef: ChangeDetectorRef,
    private applicationSectionService: ApplicationSectionsService,
    private applicationSectionElementsService: ApplicationSectionElementsService,
    private fb: FormBuilder,
    private currencyPipe: CurrencyPipe
  ) {}
  icMoreVert = icMoreVert;
  ngOnInit(): void {
    //this.getApplications();
  }

  InitTable() {
    console.log("initTable", this.ApplicationId, this.ApplicationElement);
    if (
      this.ApplicationId === undefined ||
      this.ApplicationElement === undefined
    ) {
      console.log("not ready yet");
      return false;
    }
    this.createInitialRows();
    console.log(
      "Initializing table control - ",
      this.ApplicationElement.tableSectionId,
      this.ApplicationId
    );
    this.getApplicationSectionElements(
      this.ApplicationId!,
      this.ApplicationElement.tableSectionId!
    );
  }
  getRowCount(): number {
    const rowCount =
      this._applicationElement.responses.length === 1 &&
      this._applicationElement.responses[0].intResponse
        ? this._applicationElement.responses[0].intResponse
        : this.initialRows.length;
    console.log("rowcount", rowCount);
    return rowCount;
  }
  extractRows() {
    let rows: rowTemplate[] = [];
    let numRows = this.getRowCount();
    for (let index = 0; index < numRows; index++) {
      let elements = [];
      let initialValue = this.initialRows[index];
      console.log("creating table stuff", initialValue, this.initialRows);
      if (initialValue) {
        elements.push(initialValue);
      }
      elements = elements.concat(cloneDeep(this.applicationElements));
      let data = {
        rowId: index,
        elements: elements,
      };
      rows.push(data);
    }
    this.data = rows;
  }
  getElement(elements: ApplicationElement[], elementId: number) {
    return elements.find((e) => e.elementId === elementId);
  }
  getFooterCellContents(column: any) {
    console.log("footer cell contents");

    const footerElement = this.applicationElements.find(
      (e) => e.elementId === column.elementId
    );
    console.log("footer find result");
    console.log("footer element", footerElement);
    console.log(this.data);
    if (footerElement && footerElement.sumValues) {
      let sum = 0;
      let row = this.data[0];
      if (row) {
        row.elements.forEach((element) => {
          if (element.elementId === footerElement.elementId) {
            element.responses.forEach((response) => {
              if (response.intResponse) {
                sum += response.intResponse;
              }
              if (response.currencyResponse) {
                sum += response.currencyResponse;
              }
            });
          }
        });
      }

      console.log("results of sum");
      console.log(sum);
      return sum;
    }
  }
  AddRow() {
    let elements = [];
    let rowIndex = Math.max(...this.data.map((o) => o.rowId)) + 1;
    let initialValue = this.initialRows[rowIndex];
    console.log("creating table stuff", initialValue, this.initialRows);
    if (initialValue) {
      elements.push(initialValue);
    }
    elements = elements.concat(cloneDeep(this.applicationElements));
    let data = {
      rowId: rowIndex,
      elements: elements,
    };
    this.data.push(data);
    this.SaveElement();
    this.updateTable();
  }

  SaveElement() {
    console.log("Saving Table Element ");

    let response =
      this.ApplicationElement.responses.length === 1
        ? this.ApplicationElement.responses[0]
        : {
            applicationId: this.ApplicationId,
            elementId: this.ApplicationElement.elementId,
            rowId: 0,
          };
    response.intResponse = this.data.length;
    this.ApplicationElement.responses = [response];
    this.applicationSectionElementsService
      .apiApplicationSectionElementsApplicationIdSectionSectionIdElementIdPost(
        this.ApplicationId,
        this.ApplicationElement.tableSectionId,
        this.ApplicationElement.elementId,
        response
      )
      .subscribe((result) => {
        console.log(result);
      });
  }
  getApplicationSectionElements(applicationId: number, sectionId: number) {
    console.log("getApplicationSections");
    this.applicationSectionElementsService
      .apiApplicationSectionElementsApplicationIdSectionSectionIdGet(
        applicationId,
        sectionId
      )
      .subscribe((e) => {
        console.log(e);
        this.applicationElements = e;
        this.columns = this.columns.concat(
          this.applicationElements.map((e) => {
            let result = "0";
            if (e.elementType === "Currency") {
              result = this.formatMoney(
                e.responses.reduce<number>((accumulator, obj) => {
                  return accumulator + obj.currencyResponse;
                }, 0)
              );
            }
            if (e.elementType === "Integer") {
              result = e.responses
                .reduce<number>((accumulator, obj) => {
                  return accumulator + obj.intResponse;
                }, 0)
                .toString();
            }
            console.log("this is an output from the subscribe", result);
            return {
              label: e.label,
              elementId: e.elementId,
              showFooter: e.sumValues,
              footerValue: result,
              elementType: e.elementType,
            };
          })
        );

        this.extractRows();

        console.log("table stuff", this.columns, this.data);
        this.updateTable();
      });
  }

  updateTable() {
    console.log("updating table");
    const tableSummary: TableData = {
      initialRows: this.initialRows,
      columns: this.columns,
    };
    this._tableData.next(tableSummary);
  }
  formatMoney(value) {
    const temp = `${value}`.replace(/\,/g, "");
    return this.currencyPipe.transform(temp);
  }
  isCurrency(elementType) {
    return elementType === "Currency";
  }
  handleRowChange(e) {
    console.log("cell change", e);
    this.applicationElements.forEach((element) => {
      if (element.elementId === e.elementId) {
        let response = element.responses.find((r) => r.rowId === e.rowId);
        if (response == null) {
          response = {
            applicationId: this.ApplicationId,
            elementId: element.elementId,
            rowId: e.rowId,
          };
          element.responses.push(response);
        }
        if (e.intResponse) {
          response.intResponse = e.intResponse;
        } else {
          response.intResponse = 0;
        }
        if (e.currencyResponse) {
          response.currencyResponse = e.currencyResponse;
        } else {
          response.currencyResponse = 0;
        }
        console.log("calculating footer", element.elementId);
        this.columns.forEach((column) => {
          if (
            element.elementType == "Currency" &&
            column.elementId == element.elementId
          ) {
            console.log(
              "column caculation",
              column.elementId,
              element.elementId,
              element.responses
            );
            column.footerValue = this.formatMoney(
              element.responses.reduce<number>((accumulator, obj) => {
                return accumulator + obj.currencyResponse;
              }, 0)
            );
          }
          if (
            element.elementType == "Integer" &&
            column.elementId == element.elementId
          ) {
            column.footerValue = element.responses
              .reduce<number>((accumulator, obj) => {
                return accumulator + obj.intResponse;
              }, 0)
              .toString();
          }
        });
      }
    });
    console.log(this.applicationElements);

    console.log(this.columns);
    this.changeDetectorRef.markForCheck();
  }
}
