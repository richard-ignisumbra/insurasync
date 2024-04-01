import {
  Component,
  ElementRef,
  HostBinding,
  Input,
  OnInit,
} from "@angular/core";
import {
  MSAL_GUARD_CONFIG,
  MsalBroadcastService,
  MsalGuardConfiguration,
  MsalService,
} from "@azure/msal-angular";

import { AddContactComponent } from "../../../app/pages/pages/contacts/add-contact/add-contact.component";
import { AppUtilityService } from "../../../app/shared-utilities/app-utility.service";
import { ConfigService } from "../../services/config.service";
import { LayoutService } from "../../services/layout.service";
import { MegaMenuComponent } from "../../components/mega-menu/mega-menu.component";
import { NavigationService } from "../../services/navigation.service";
import { PopoverService } from "../../components/popover/popover.service";
import emojioneDE from "@iconify/icons-emojione/flag-for-flag-germany";
import emojioneUS from "@iconify/icons-emojione/flag-for-flag-united-states";
import icArrowDropDown from "@iconify/icons-ic/twotone-arrow-drop-down";
import icAssignment from "@iconify/icons-ic/twotone-assignment";
import icAssignmentTurnedIn from "@iconify/icons-ic/twotone-assignment-turned-in";
import icBallot from "@iconify/icons-ic/twotone-ballot";
import icBookmarks from "@iconify/icons-ic/twotone-bookmarks";
import icDescription from "@iconify/icons-ic/twotone-description";
import icDomain from "@iconify/icons-ic/twotone-domain";
import icDoneAll from "@iconify/icons-ic/twotone-done-all";
import icMenu from "@iconify/icons-ic/twotone-menu";
import icPersonAdd from "@iconify/icons-ic/twotone-person-add";
import icReceipt from "@iconify/icons-ic/twotone-receipt";
import icSearch from "@iconify/icons-ic/twotone-search";
import { map } from "rxjs/operators";

@Component({
  selector: "vex-toolbar",
  templateUrl: "./toolbar.component.html",
  styleUrls: ["./toolbar.component.scss"],
})
export class ToolbarComponent implements OnInit {
  @Input() mobileQuery: boolean;

  @Input()
  @HostBinding("class.shadow-b")
  hasShadow: boolean;

  navigationItems = this.navigationService.items;

  isHorizontalLayout$ = this.configService.config$.pipe(
    map((config) => config.layout === "horizontal")
  );
  isVerticalLayout$ = this.configService.config$.pipe(
    map((config) => config.layout === "vertical")
  );
  isNavbarInToolbar$ = this.configService.config$.pipe(
    map((config) => config.navbar.position === "in-toolbar")
  );
  isNavbarBelowToolbar$ = this.configService.config$.pipe(
    map((config) => config.navbar.position === "below-toolbar")
  );
  loginDisplay = false;
  icSearch = icSearch;
  icBookmarks = icBookmarks;
  emojioneUS = emojioneUS;
  emojioneDE = emojioneDE;
  icMenu = icMenu;
  icPersonAdd = icPersonAdd;
  icAssignmentTurnedIn = icAssignmentTurnedIn;
  icBallot = icBallot;
  icDescription = icDescription;
  icAssignment = icAssignment;
  icReceipt = icReceipt;
  icDoneAll = icDoneAll;
  icDomain = icDomain;
  icArrowDropDown = icArrowDropDown;

  constructor(
    private layoutService: LayoutService,
    private configService: ConfigService,
    private navigationService: NavigationService,
    private popoverService: PopoverService,
    private authService: MsalService,
    public appUtilityService: AppUtilityService
  ) {}

  ngOnInit() {
    this.setLoginDisplay();
    this.appUtilityService.retrieveUserProfile();
    this.appUtilityService.UserProfile$.subscribe((data) => {});
  }

  setLoginDisplay() {
    console.log("this is the setlogindisplay function");
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    if (this.loginDisplay) {
      let account = this.authService.instance.getAllAccounts()[0];
    }
  }
  openQuickpanel() {
    this.layoutService.openQuickpanel();
  }

  openSidenav() {
    this.layoutService.openSidenav();
  }

  openMegaMenu(origin: ElementRef | HTMLElement) {
    this.popoverService.open({
      content: AddContactComponent,
      origin,
      position: [
        {
          originX: "start",
          originY: "bottom",
          overlayX: "start",
          overlayY: "top",
        },
        {
          originX: "end",
          originY: "bottom",
          overlayX: "end",
          overlayY: "top",
        },
      ],
    });
  }

  openSearch() {
    this.layoutService.openSearch();
  }
}
