import { CommonModule } from "@angular/common";
import { FlexLayoutModule } from "@angular/flex-layout";
import { HomePageComponent } from "./homePage.component";
import { HomePageRoutingModule } from "./homePage-routing.module";
import { IconModule } from "@visurel/iconify-angular";
import { MatButtonModule } from "@angular/material/button";
import { NgModule } from "@angular/core";

@NgModule({
  declarations: [HomePageComponent],
  imports: [
    CommonModule,
    HomePageRoutingModule,
    IconModule,
    FlexLayoutModule,
    MatButtonModule,
  ],
})
export class HomePageModule {}
