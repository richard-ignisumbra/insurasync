import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { AddAttachmentComponent } from "./add-attachment/add-attachment.component";
import { AddMemberComponent } from "./add-member/add-member.component";
import { AddNoteComponent } from "./add-note/add-note.component";
import { BreadcrumbsModule } from "../../../../@vex/components/breadcrumbs/breadcrumbs.module";
import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ContactsModule } from "../contacts/contacts.module";
import { ContainerModule } from "../../../../@vex/directives/container/container.module";
import { FlexLayoutModule } from "@angular/flex-layout";
import { IconModule } from "@visurel/iconify-angular";
import { MatButtonModule } from "@angular/material/button";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatNativeDateModule } from "@angular/material/core";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatRadioModule } from "@angular/material/radio";
import { MatSelectModule } from "@angular/material/select";
import { MatTabsModule } from "@angular/material/tabs";
import { MemberAttachmentComponent } from "./member-attachments/member-attachments.component";
import { MemberDetailsComponent } from "./member-details/member-details.component";
import { MemberListComponent } from "./member-list/member-list.component";
import { MemberNotesComponent } from "./member-notes/member-notes.component";
import { MemberTabbedComponent } from "./member-tabbed/member-tabbed.component";
import { MembersRoutingModule } from "./members-routing.module";
import { NgModule } from "@angular/core";
import { NgxDatatableModule } from "@swimlane/ngx-datatable";
import { PageLayoutModule } from "../../../../@vex/components/page-layout/page-layout.module";
import { SecondaryToolbarModule } from "../../../../@vex/components/secondary-toolbar/secondary-toolbar.module";
import { SharedUtilitiesModule } from "../../../shared-utilities/shared-utilities.module";
import { SidebarModule } from "../../../../@vex/components/sidebar/sidebar.module";

@NgModule({
  declarations: [
    MemberListComponent,
    MemberDetailsComponent,
    AddMemberComponent,
    MemberTabbedComponent,
    MemberNotesComponent,
    MemberAttachmentComponent,
    AddNoteComponent,
    AddAttachmentComponent,
  ],
  imports: [
    CommonModule,
    MembersRoutingModule,
    IconModule,
    FlexLayoutModule,
    NgxDatatableModule,
    MatButtonModule,
    BreadcrumbsModule,
    SidebarModule,
    SecondaryToolbarModule,
    MatFormFieldModule,
    MatRadioModule,
    MatProgressBarModule,
    MatInputModule,
    MatCheckboxModule,
    MatSelectModule,
    ReactiveFormsModule,
    MatNativeDateModule,
    MatIconModule,
    PageLayoutModule,
    MatTabsModule,
    FormsModule,
    MatDatepickerModule,
    ContactsModule,
    SharedUtilitiesModule,
  ],
})
export class MembersModule {}
