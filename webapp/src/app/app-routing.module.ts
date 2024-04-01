import { ExtraOptions, RouterModule, Routes } from "@angular/router";

import { CustomLayoutComponent } from "./custom-layout/custom-layout.component";
import { ErrorComponent } from "./error/error.component";
import { GuardedComponent } from "./guarded/guarded.component";
import { MsalGuard } from "@azure/msal-angular";
import { NgModule } from "@angular/core";
import { PermaLayoutComponent } from "./perma-layout/perma-layout.component";
import { ProfileComponent } from "./profile/profile.component";
import { PublicHomeComponent } from "./public-home/public-home.component";

const routes: Routes = [
  {
    path: "guarded",
    component: GuardedComponent,
    canActivate: [MsalGuard],
  },
  {
    path: "profile",
    component: ProfileComponent,
    // The profile component is protected with MSAL Guard.
    canActivate: [MsalGuard],
  },
  {
    path: "contacts",
    component: PermaLayoutComponent,
    loadChildren: () =>
      import("./pages/pages/contacts/contacts.module").then(
        (m) => m.ContactsModule
      ),
    canActivate: [MsalGuard],
  },
  {
    path: "members",
    component: PermaLayoutComponent,
    loadChildren: () =>
      import("./pages/pages/members/members.module").then(
        (m) => m.MembersModule
      ),
    canActivate: [MsalGuard],
  },
  {
    path: "applications",
    component: PermaLayoutComponent,
    loadChildren: () =>
      import("./pages/pages/applications/applications.module").then(
        (m) => m.ApplicationsModule
      ),
    canActivate: [MsalGuard],
  },
  {
    // Needed for hash routing
    path: "error",
    component: ErrorComponent,
  },
  {
    // Needed for hash routing
    path: "state",
    component: PublicHomeComponent,
  },

  {
    // Needed for hash routing
    path: "code",
    component: PublicHomeComponent,
  },
  {
    path: "",
    component: PermaLayoutComponent,
    loadChildren: () =>
      import("./pages/pages/homePage/homePage.module").then(
        (m) => m.HomePageModule
      ),
  },
];

const isIframe = window !== window.parent && !window.opener;

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      // preloadingStrategy: PreloadAllModules,
      scrollPositionRestoration: "enabled",
      relativeLinkResolution: "corrected",
      anchorScrolling: "enabled",
      initialNavigation: "enabled",
      paramsInheritanceStrategy: "always",
      enableTracing: false,
      useHash: false,
    }),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}
