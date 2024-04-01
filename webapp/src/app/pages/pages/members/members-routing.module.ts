import { RouterModule, Routes } from "@angular/router";

import { AddMemberComponent } from "./add-member/add-member.component";
import { MemberListComponent } from "./member-list/member-list.component";
import { MemberTabbedComponent } from "./member-tabbed/member-tabbed.component";
import { NgModule } from "@angular/core";
import { QuicklinkModule } from "ngx-quicklink";

const routes: Routes = [
  {
    path: "",
    component: MemberListComponent,
  },
  {
    path: "details/:id",
    component: MemberTabbedComponent,
  },

  {
    path: "addMember",
    component: AddMemberComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule],
})
export class MembersRoutingModule {}
