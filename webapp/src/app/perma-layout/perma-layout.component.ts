import { Component, OnInit, ViewChild } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { filter, map, startWith } from "rxjs/operators";

import { BreakpointObserver } from "@angular/cdk/layout";
import { ConfigService } from "../../@vex/services/config.service";
import { LayoutService } from "../../@vex/services/layout.service";
import { SidebarComponent } from "../../@vex/components/sidebar/sidebar.component";
import { checkRouterChildsData } from "../../@vex/utils/check-router-childs-data";

@UntilDestroy()
@Component({
  selector: "vex-custom-layout",
  templateUrl: "./perma-layout.component.html",
  styleUrls: ["./perma-layout.component.scss"],
})
export class PermaLayoutComponent implements OnInit {
  sidenavCollapsed$ = this.layoutService.sidenavCollapsed$;
  isFooterVisible$ = this.configService.config$.pipe(
    map((config) => config.footer.visible)
  );
  isDesktop$ = this.layoutService.isDesktop$;

  toolbarShadowEnabled$ = this.router.events.pipe(
    filter((event) => event instanceof NavigationEnd),
    startWith(null),
    map(() =>
      checkRouterChildsData(
        this.router.routerState.root.snapshot,
        (data) => data.toolbarShadowEnabled
      )
    )
  );

  @ViewChild("configpanel", { static: true }) configpanel: SidebarComponent;

  constructor(
    private layoutService: LayoutService,
    private configService: ConfigService,
    private breakpointObserver: BreakpointObserver,
    private router: Router
  ) {}

  ngOnInit() {
    this.layoutService.configpanelOpen$
      .pipe(untilDestroyed(this))
      .subscribe((open) =>
        open ? this.configpanel.open() : this.configpanel.close()
      );
  }
}
