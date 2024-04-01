import { RouterModule, Routes } from "@angular/router";

import { AddContactComponent } from "./add-contact/add-contact.component";
import { NgModule } from "@angular/core";
import { QuicklinkModule } from "ngx-quicklink";

const routes: Routes = [
  {
    path: "addContact",
    component: AddContactComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule],
})
export class MembersRoutingModule {}
