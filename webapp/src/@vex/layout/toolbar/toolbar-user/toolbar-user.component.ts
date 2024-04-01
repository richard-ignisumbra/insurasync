import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  Inject,
  OnDestroy,
} from "@angular/core";
import { PopoverService } from "../../../components/popover/popover.service";
import { ToolbarUserDropdownComponent } from "./toolbar-user-dropdown/toolbar-user-dropdown.component";
import icPerson from "@iconify/icons-ic/twotone-person";
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

import { b2cPolicies } from "../../../../app/auth-config";
@Component({
  selector: "vex-toolbar-user",
  templateUrl: "./toolbar-user.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToolbarUserComponent implements OnInit, OnDestroy {
  dropdownOpen: boolean;
  icPerson = icPerson;
  loginDisplay = false;
  private readonly _destroying$ = new Subject<void>();
  public displayName: string;
  constructor(
    private popover: PopoverService,
    private cd: ChangeDetectorRef,
    @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private authService: MsalService,
    private msalBroadcastService: MsalBroadcastService
  ) {}

  ngOnInit() {
    console.log("this is the init from toolbar-user.component");
    this.setLoginDisplay();
  }

  setLoginDisplay() {
    console.log(
      "this is the setlogindisplay function from the button",
      this.authService.instance.getActiveAccount()
    );
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    if (this.loginDisplay) {
      let account = this.authService.instance.getAllAccounts()[0];
      this.displayName = account.name;
      this.cd.detectChanges();
    }
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
            this.cd.detectChanges();
          });
      } else {
        this.authService
          .loginPopup(userFlowRequest)
          .subscribe((response: AuthenticationResult) => {
            this.authService.instance.setActiveAccount(response.account);
            this.cd.detectChanges();
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

  editProfile() {
    let editProfileFlowRequest = {
      scopes: ["openid"],
      authority: b2cPolicies.authorities.editProfile.authority,
    };

    this.login(editProfileFlowRequest);
  }

  ngOnDestroy(): void {
    this._destroying$.next(undefined);
    this._destroying$.complete();
  }

  showPopover(originRef: HTMLElement) {
    this.dropdownOpen = true;
    this.cd.markForCheck();

    const popoverRef = this.popover.open({
      content: ToolbarUserDropdownComponent,
      origin: originRef,
      offsetY: 12,
      position: [
        {
          originX: "center",
          originY: "top",
          overlayX: "center",
          overlayY: "bottom",
        },
        {
          originX: "end",
          originY: "bottom",
          overlayX: "end",
          overlayY: "top",
        },
      ],
    });

    popoverRef.afterClosed$.subscribe(() => {
      this.dropdownOpen = false;
      this.cd.markForCheck();
    });
  }
}
