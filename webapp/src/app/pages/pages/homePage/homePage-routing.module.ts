import { RouterModule, Routes } from "@angular/router";

import { HomePageComponent } from "./homePage.component";
import { NgModule } from "@angular/core";
import { QuicklinkModule } from "ngx-quicklink";

const routes: Routes = [
  {
    path: "",
    component: HomePageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule],
})
export class HomePageRoutingModule {}
