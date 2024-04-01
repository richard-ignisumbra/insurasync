import { CommonModule } from "@angular/common";
import { ConfigPanelModule } from "../../@vex/components/config-panel/config-panel.module";
import { FooterModule } from "../../@vex/layout/footer/footer.module";
import { LayoutModule } from "../../@vex/layout/layout.module";
import { NgModule } from "@angular/core";
import { PermaLayoutComponent } from "./perma-layout.component";
import { QuickpanelModule } from "../../@vex/layout/quickpanel/quickpanel.module";
import { SidebarModule } from "../../@vex/components/sidebar/sidebar.module";
import { SidenavModule } from "../../@vex/layout/sidenav/sidenav.module";
import { ToolbarModule } from "../../@vex/layout/toolbar/toolbar.module";

@NgModule({
  declarations: [PermaLayoutComponent],
  imports: [
    CommonModule,
    LayoutModule,
    SidenavModule,
    ToolbarModule,
    FooterModule,
    ConfigPanelModule,
    SidebarModule,
    QuickpanelModule,
  ],
})
export class PermaLayoutModule {}
