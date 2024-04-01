import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  Inject,
} from "@angular/core";

import { Icon } from "@visurel/iconify-angular";
import { MenuItem } from "../interfaces/menu-item.interface";
import { PopoverRef } from "../../../../components/popover/popover-ref";
import icAccessTime from "@iconify/icons-ic/twotone-access-time";
import icAccountCircle from "@iconify/icons-ic/twotone-account-circle";
import icArrowDropDown from "@iconify/icons-ic/twotone-arrow-drop-down";
import icBusiness from "@iconify/icons-ic/twotone-business";
import icCheckCircle from "@iconify/icons-ic/twotone-check-circle";
import icChevronRight from "@iconify/icons-ic/twotone-chevron-right";
import icDoNotDisturb from "@iconify/icons-ic/twotone-do-not-disturb";
import icListAlt from "@iconify/icons-ic/twotone-list-alt";
import icLock from "@iconify/icons-ic/twotone-lock";
import icMoveToInbox from "@iconify/icons-ic/twotone-move-to-inbox";
import icNotificationsOff from "@iconify/icons-ic/twotone-notifications-off";
import icOfflineBolt from "@iconify/icons-ic/twotone-offline-bolt";
import icPerson from "@iconify/icons-ic/twotone-person";
import icSettings from "@iconify/icons-ic/twotone-settings";
import icTableChart from "@iconify/icons-ic/twotone-table-chart";
import icVerifiedUser from "@iconify/icons-ic/twotone-verified-user";
import { trackById } from "../../../../utils/track-by";
import {
  MsalService,
  MsalBroadcastService,
  MSAL_GUARD_CONFIG,
  MsalGuardConfiguration,
} from "@azure/msal-angular";
import { filter, map, takeUntil } from "rxjs/operators";
import {
  EventMessage,
  EventType,
  InteractionType,
  InteractionStatus,
  PopupRequest,
  RedirectRequest,
  AuthenticationResult,
  AuthError,
} from "@azure/msal-browser";
import { Subject } from "rxjs";

import { b2cPolicies } from "../../../../../app/auth-config";
export interface OnlineStatus {
  id: "online" | "away" | "dnd" | "offline";
  label: string;
  icon: Icon;
  colorClass: string;
}

@Component({
  selector: "vex-toolbar-user-dropdown",
  templateUrl: "./toolbar-user-dropdown.component.html",
  styleUrls: ["./toolbar-user-dropdown.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToolbarUserDropdownComponent implements OnInit {
  public displayName: string;
  loginDisplay = false;
  items: MenuItem[] = [
    {
      id: "1",
      icon: icAccountCircle,
      label: "My Profile",
      description: "Personal Information",
      colorClass: "text-teal",
      route: "/apps/social",
    },
  ];

  statuses: OnlineStatus[] = [
    {
      id: "online",
      label: "Online",
      icon: icCheckCircle,
      colorClass: "text-green",
    },
    {
      id: "away",
      label: "Away",
      icon: icAccessTime,
      colorClass: "text-orange",
    },
    {
      id: "dnd",
      label: "Do not disturb",
      icon: icDoNotDisturb,
      colorClass: "text-red",
    },
    {
      id: "offline",
      label: "Offline",
      icon: icOfflineBolt,
      colorClass: "text-gray",
    },
  ];

  activeStatus: OnlineStatus = this.statuses[0];

  trackById = trackById;
  icPerson = icPerson;
  icSettings = icSettings;
  icChevronRight = icChevronRight;
  icArrowDropDown = icArrowDropDown;
  icBusiness = icBusiness;
  icVerifiedUser = icVerifiedUser;
  icLock = icLock;
  icNotificationsOff = icNotificationsOff;

  constructor(
    private cd: ChangeDetectorRef,
    private popoverRef: PopoverRef<ToolbarUserDropdownComponent>,
    @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private authService: MsalService,
    private msalBroadcastService: MsalBroadcastService
  ) {}

  ngOnInit() {
    this.setLoginDisplay();
  }

  setLoginDisplay() {
    console.log("this is the setlogindisplay function");
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    if (this.loginDisplay) {
      let account = this.authService.instance.getAllAccounts()[0];
      this.displayName = account.name;
    }
  }

  setStatus(status: OnlineStatus) {
    this.activeStatus = status;
    this.cd.markForCheck();
  }

  close() {
    this.popoverRef.close();
  }
  editProfile() {
    let editProfileFlowRequest = {
      scopes: ["openid"],
      authority: b2cPolicies.authorities.editProfile.authority,
    };

    this.login(editProfileFlowRequest);
  }

  login(userFlowRequest?: RedirectRequest | PopupRequest) {
    if (this.msalGuardConfig.interactionType === InteractionType.Popup) {
      if (this.msalGuardConfig.authRequest) {
        this.authService
          .loginPopup({
            ...this.msalGuardConfig.authRequest,
            ...userFlowRequest,
          } as PopupRequest)
          .subscribe((response: AuthenticationResult) => {
            this.authService.instance.setActiveAccount(response.account);
          });
      } else {
        this.authService
          .loginPopup(userFlowRequest)
          .subscribe((response: AuthenticationResult) => {
            this.authService.instance.setActiveAccount(response.account);
          });
      }
    } else {
      if (this.msalGuardConfig.authRequest) {
        this.authService.loginRedirect({
          ...this.msalGuardConfig.authRequest,
          ...userFlowRequest,
        } as RedirectRequest);
      } else {
        this.authService.loginRedirect(userFlowRequest);
      }
    }
  }

  logout() {
    let redirectURI;
    if (location.hostname === "localhost") {
      redirectURI = "http://localhost:3200";
    } else {
      redirectURI = b2cPolicies.productionLogOutRedirect;
    }
    this.authService.logoutRedirect({
      postLogoutRedirectUri: redirectURI,
    });
  }
}
