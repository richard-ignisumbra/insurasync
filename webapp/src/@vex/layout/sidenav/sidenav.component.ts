import { Component, Input, OnInit } from "@angular/core";
import {
  MSAL_GUARD_CONFIG,
  MsalBroadcastService,
  MsalGuardConfiguration,
  MsalService,
} from "@azure/msal-angular";

import { ConfigService } from "../../services/config.service";
import { LayoutService } from "../../services/layout.service";
import { NavigationService } from "../../services/navigation.service";
import icRadioButtonChecked from "@iconify/icons-ic/twotone-radio-button-checked";
import icRadioButtonUnchecked from "@iconify/icons-ic/twotone-radio-button-unchecked";
import { map } from "rxjs/operators";
import { trackByRoute } from "../../utils/track-by";

@Component({
  selector: "vex-sidenav",
  templateUrl: "./sidenav.component.html",
  styleUrls: ["./sidenav.component.scss"],
})
export class SidenavComponent implements OnInit {
  @Input() collapsed: boolean;
  collapsedOpen$ = this.layoutService.sidenavCollapsedOpen$;
  title$ = this.configService.config$.pipe(
    map((config) => config.sidenav.title)
  );
  imageUrl$ = this.configService.config$.pipe(
    map((config) => config.sidenav.imageUrl)
  );
  showCollapsePin$ = this.configService.config$.pipe(
    map((config) => config.sidenav.showCollapsePin)
  );
  loginDisplay = false;
  items = this.navigationService.items;
  trackByRoute = trackByRoute;
  icRadioButtonChecked = icRadioButtonChecked;
  icRadioButtonUnchecked = icRadioButtonUnchecked;

  constructor(
    private navigationService: NavigationService,
    private layoutService: LayoutService,
    private configService: ConfigService,
    private authService: MsalService
  ) {}

  ngOnInit() {
    this.setLoginDisplay();
  }

  setLoginDisplay() {
    console.log("this is the setlogindisplay function");
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    if (this.loginDisplay) {
      let account = this.authService.instance.getAllAccounts()[0];
    }
  }
  onMouseEnter() {
    this.layoutService.collapseOpenSidenav();
  }

  onMouseLeave() {
    this.layoutService.collapseCloseSidenav();
  }

  toggleCollapse() {
    this.collapsed
      ? this.layoutService.expandSidenav()
      : this.layoutService.collapseSidenav();
  }
}
