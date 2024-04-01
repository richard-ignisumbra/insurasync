import { RouterModule, Routes } from "@angular/router";

import { ApplicationListComponent } from "./application-list/application-list.component";
import { ApplicationReportsComponent } from "./application-reports/application-reports.component";
import { NgModule } from "@angular/core";
import { QuicklinkModule } from "ngx-quicklink";
import { ViewApplicationComponent } from "./view-application/view-application.component";
import { ViewApplicationSectionComponent } from "./view-application-section/view-application-section.component";

const routes: Routes = [
  {
    path: "",
    component: ApplicationListComponent,
    data: {
      toolbarShadowEnabled: true,
      scrollDisabled: true,
    },
  },
  {
    path: "reports",
    component: ApplicationReportsComponent,
    data: {
      toolbarShadowEnabled: true,
      scrollDisabled: true,
    },
  },
  {
    path: "details/:id",
    component: ViewApplicationComponent,
    children: [
      {
        path: "",
        component: ViewApplicationSectionComponent,
      },
      {
        path: ":sectionId",
        component: ViewApplicationSectionComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule],
})
export class MembersRoutingModule {}
