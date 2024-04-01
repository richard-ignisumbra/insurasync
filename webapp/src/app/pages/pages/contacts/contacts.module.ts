import {} from "./member-contact-details/member-contact-details.component";

import { AddContactComponent } from "./add-contact/add-contact.component";
import { AddExistingContactComponent } from "./add-existing-contact/add-existing-contact.component";
import { BreadcrumbsModule } from "../../../../@vex/components/breadcrumbs/breadcrumbs.module";
import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { FlexLayoutModule } from "@angular/flex-layout";
import { FormsModule } from "@angular/forms";
import { IconModule } from "@visurel/iconify-angular";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatButtonModule } from "@angular/material/button";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatDialogModule } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatNativeDateModule } from "@angular/material/core";
import { MatSelectModule } from "@angular/material/select";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MemberContactDetailsComponent } from "./member-contact-details/member-contact-details.component";
import { MemberContactListComponent } from "./member-contact-list/member-contact-list.component";
import { MembersRoutingModule } from "./contacts-routing.module";
import { NgModule } from "@angular/core";
import { NgxDatatableModule } from "@swimlane/ngx-datatable";
import { ReactiveFormsModule } from "@angular/forms";
import { SecondaryToolbarModule } from "../../../../@vex/components/secondary-toolbar/secondary-toolbar.module";
import { SidebarModule } from "../../../../@vex/components/sidebar/sidebar.module";

@NgModule({
  declarations: [
    AddContactComponent,
    MemberContactListComponent,
    MemberContactDetailsComponent,
    AddExistingContactComponent,
  ],
  imports: [
    CommonModule,
    MembersRoutingModule,
    IconModule,
    FlexLayoutModule,
    NgxDatatableModule,
    MatButtonModule,
    MatAutocompleteModule,
    BreadcrumbsModule,
    SidebarModule,
    SecondaryToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatSelectModule,
    MatDialogModule,
    MatSnackBarModule,
    MatNativeDateModule,
    FormsModule,
    ReactiveFormsModule,
    MatDatepickerModule,
  ],
  exports: [
    MemberContactListComponent,
    MemberContactDetailsComponent,
    AddExistingContactComponent,
  ],
})
export class ContactsModule {}
