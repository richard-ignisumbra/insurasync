import {
  ApiModule,
  Configuration,
  ConfigurationParameters,
} from "./core/api/v1";
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import {
  IPublicClientApplication,
  InteractionType,
  PublicClientApplication,
} from "@azure/msal-browser";
import {
  ISWIsLoadingDirectiveConfig,
  IsLoadingModule,
  SW_IS_LOADING_DIRECTIVE_CONFIG,
} from "@service-work/is-loading";
import {
  MSAL_GUARD_CONFIG,
  MSAL_INSTANCE,
  MsalBroadcastService,
  MsalGuard,
  MsalGuardConfiguration,
  MsalInterceptor,
  MsalModule,
  MsalRedirectComponent,
  MsalService,
} from "@azure/msal-angular";
import { msalConfig, protectedResources } from "./auth-config";

import { AppComponent } from "./app.component";
import { AppRoutingModule } from "./app-routing.module";
import { ApplicationComponent } from "./application/application.component";
import { AuthInterceptor } from "./auth.interceptor";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { BrowserModule } from "@angular/platform-browser";
import { CustomLayoutModule } from "./custom-layout/custom-layout.module";
import { ErrorComponent } from "./error/error.component";
import { GuardedComponent } from "./guarded/guarded.component";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatListModule } from "@angular/material/list";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatTableModule } from "@angular/material/table";
import { MatToolbarModule } from "@angular/material/toolbar";
import { NgModule } from "@angular/core";
import { PermaLayoutModule } from "./perma-layout/perma-layout.module";
import { ProfileComponent } from "./profile/profile.component";
import { PublicHomeComponent } from "./public-home/public-home.component";
import { SharedUtilitiesModule } from "./shared-utilities/shared-utilities.module";
import { VexModule } from "../@vex/vex.module";
import { environment } from "src/environments/environment";

/**
 * Here we pass the configuration parameters to create an MSAL instance.
 * For more info, visit: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-angular/docs/v2-docs/configuration.md
 */
export function MSALInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication(msalConfig);
}

/**
 * Set your default interaction type for MSALGuard here. If you have any
 * additional scopes you want the user to consent upon login, add them here as well.
 */
export function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Popup,
  };
}

export function apiConfigFactory(): Configuration {
  const params: ConfigurationParameters = {
    basePath: environment.apiEndpoint,
  };
  return new Configuration(params);
}
const myConfig: ISWIsLoadingDirectiveConfig = {
  // disable element while loading (default: true)
  disableEl: true,
  // the class used to indicate loading (default: "sw-is-loading")
  loadingClass: "sw-is-loading",
  // should a spinner element be added to the dom
  // (default: varies --> true for button/anchor elements, false otherwise)
  addSpinnerEl: true,
};
@NgModule({
  declarations: [
    AppComponent,
    GuardedComponent,
    ProfileComponent,
    PublicHomeComponent,
    ErrorComponent,
    ApplicationComponent,
  ],

  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatButtonModule,
    IsLoadingModule,
    MatProgressBarModule,
    MatToolbarModule,
    MatListModule,
    SharedUtilitiesModule,
    MatTableModule,
    MatCardModule,
    HttpClientModule,
    [ApiModule.forRoot(apiConfigFactory)],
    // Initiate the MSAL library with the MSAL configuration object
    MsalModule.forRoot(
      new PublicClientApplication(msalConfig),
      {
        // The routing guard configuration.
        interactionType: InteractionType.Popup,
        authRequest: {
          scopes: protectedResources.membersApi.scopes,
        },
      },
      {
        // MSAL interceptor configuration.
        // The protected resource mapping maps your web API with the corresponding app scopes. If your code needs to call another web API, add the URI mapping here.
        interactionType: InteractionType.Popup,
        protectedResourceMap: new Map([
          [
            protectedResources.membersApi.endpoint,
            protectedResources.membersApi.scopes,
          ],
          [
            protectedResources.applicationSectionElementsApi.endpoint,
            protectedResources.applicationSectionElementsApi.scopes,
          ],
          [
            protectedResources.applicationFileUploadServiceApi.endpoint,
            protectedResources.applicationFileUploadServiceApi.scopes,
          ],
          [
            protectedResources.applicationSectionsApi.endpoint,
            protectedResources.applicationSectionsApi.scopes,
          ],
          [
            protectedResources.applicationsApi.endpoint,
            protectedResources.applicationsApi.scopes,
          ],

          [
            protectedResources.linesApi.endpoint,
            protectedResources.linesApi.scopes,
          ],

          [
            protectedResources.contactsApi.endpoint,
            protectedResources.contactsApi.scopes,
          ],
          [
            protectedResources.categoriesApi.endpoint,
            protectedResources.categoriesApi.scopes,
          ],
          [
            protectedResources.memberFileUploadServiceApi.endpoint,
            protectedResources.memberFileUploadServiceApi.scopes,
          ],
          [
            protectedResources.applicationTypesApi.endpoint,
            protectedResources.applicationTypesApi.scopes,
          ],
        ]),
      }
    ),
    // Vex
    VexModule,
    CustomLayoutModule,
    PermaLayoutModule,
  ],
  providers: [
    { provide: SW_IS_LOADING_DIRECTIVE_CONFIG, useValue: myConfig },
    /* Changes start here. */
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
    },
    {
      provide: MSAL_GUARD_CONFIG,
      useFactory: MSALGuardConfigFactory,
    },
    MsalService,
    MsalGuard,
    MsalBroadcastService,
  ],
  bootstrap: [AppComponent, MsalRedirectComponent],
})
export class AppModule {}
