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
  Input,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from "@angular/core";
import { MatDrawer, MatDrawerMode } from "@angular/material/sidenav";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";

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
import { Observable } from "rxjs";
import { fadeInRight400ms } from "../../../../../@vex/animations/fade-in-right.animation";
import icEdit from "@iconify/icons-ic/twotone-edit";
import icMail from "@iconify/icons-ic/twotone-mail";
import icMenu from "@iconify/icons-ic/twotone-menu";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSearch from "@iconify/icons-ic/twotone-search";
import { map } from "rxjs/operators";
import { protectedResources } from "../../../../auth-config";
import { scaleIn400ms } from "../../../../../@vex/animations/scale-in.animation";

@Component({
  selector: "view-element-text",
  templateUrl: "./view-element-text.component.html",
  styleUrls: ["./view-element-text.component.scss"],
  animations: [scaleIn400ms, fadeInRight400ms],
  encapsulation: ViewEncapsulation.None,
})
export class ViewElementTextComponent implements OnInit {
  @Input() applicationElement: ApplicationElement;
  ElementTypeEnum = ElementType;
  isDesktop$ = this.layoutService.isDesktop$;
  ltLg$ = this.layoutService.ltLg$;
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
}
