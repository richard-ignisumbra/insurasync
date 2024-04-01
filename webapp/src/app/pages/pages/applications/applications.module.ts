import { ApplicationListComponent } from "./application-list/application-list.component";
import { ApplicationReportsComponent } from "./application-reports/application-reports.component";
import { ApplicationSideMenuComponent } from "./application-side-menu/application-side-menu.component";
import { ApplicationSidenavLinkComponent } from "./application-sidenav-link/application-sidenav-link.component";
import { BreadcrumbsModule } from "../../../../@vex/components/breadcrumbs/breadcrumbs.module";
import { CommonModule } from "@angular/common";
import { CreateApplicationComponent } from "./create-application/create-application.component";
import { CurrencyMaskModule } from "ng2-currency-mask";
import { CurrencyPipe } from "@angular/common";
import { FlexLayoutModule } from "@angular/flex-layout";
import { FormsModule } from "@angular/forms";
import { IconModule } from "@visurel/iconify-angular";
import { IsLoadingModule } from "@service-work/is-loading";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatButtonModule } from "@angular/material/button";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatDialogModule } from "@angular/material/dialog";
import { MatDividerModule } from "@angular/material/divider";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatNativeDateModule } from "@angular/material/core";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatRadioModule } from "@angular/material/radio";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTableModule } from "@angular/material/table";
import { MatTabsModule } from "@angular/material/tabs";
import { MembersRoutingModule } from "./application-routing.module";
import { NgModule } from "@angular/core";
import { NgxDatatableModule } from "@swimlane/ngx-datatable";
import { ReactiveFormsModule } from "@angular/forms";
import { ScrollbarModule } from "../../../../@vex/components/scrollbar/scrollbar.module";
import { SecondaryToolbarModule } from "../../../../@vex/components/secondary-toolbar/secondary-toolbar.module";
import { SharedUtilitiesModule } from "../../../shared-utilities/shared-utilities.module";
import { SubmitApplicationComponent } from "./submit-application/submit-application.component";
import { ViewApplicationComponent } from "./view-application/view-application.component";
import { ViewApplicationSectionComponent } from "./view-application-section/view-application-section.component";
import { ViewElementComponent } from "./view-element/view-element.component";
import { ViewElementFileAttachmentComponent } from "./view-element-fileattachment/view-element-fileattachment.component";
import { ViewElementHTMLComponent } from "./view-element-html/view-element-html.component";
import { ViewElementTableComponent } from "./view-element-table/view-element-table.component";
import { ViewElementTextComponent } from "./view-element-text/view-element-text.component";
@NgModule({
  declarations: [
    ApplicationListComponent,
    CreateApplicationComponent,
    ViewApplicationComponent,
    ApplicationSideMenuComponent,
    ApplicationSidenavLinkComponent,
    ViewApplicationSectionComponent,
    ViewElementFileAttachmentComponent,
    ViewElementHTMLComponent,
    ViewElementTextComponent,
    ViewElementTableComponent,
    ViewElementComponent,
    SubmitApplicationComponent,
    ApplicationReportsComponent,
  ],
  providers: [CurrencyPipe],
  imports: [
    CommonModule,
    MembersRoutingModule,
    IconModule,
    IsLoadingModule,
    FlexLayoutModule,
    MatButtonModule,
    MatDividerModule,
    SecondaryToolbarModule,
    MatFormFieldModule,
    BreadcrumbsModule,
    ScrollbarModule,
    CurrencyMaskModule,
    MatProgressBarModule,
    MatSidenavModule,
    MatInputModule,
    MatRadioModule,
    MatTabsModule,
    SharedUtilitiesModule,
    MatSelectModule,
    FormsModule,
    MatSnackBarModule,
    MatNativeDateModule,
    ReactiveFormsModule,
    NgxDatatableModule,
    MatDialogModule,
    MatDatepickerModule,
    MatAutocompleteModule,
    MatTableModule,
  ],
})
export class ApplicationsModule {}
