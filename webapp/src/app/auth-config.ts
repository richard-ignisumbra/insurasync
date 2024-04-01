/**
 * This file contains authentication parameters. Contents of this file
 * is roughly the same across other MSAL.js libraries. These parameters
 * are used to initialize Angular and MSAL Angular configurations in
 * in app.module.ts file.
 */

import {
  BrowserCacheLocation,
  Configuration,
  LogLevel,
} from "@azure/msal-browser";

import { environment } from "src/environments/environment";

const isIE =
  window.navigator.userAgent.indexOf("MSIE ") > -1 ||
  window.navigator.userAgent.indexOf("Trident/") > -1;

/**
 * Enter here the user flows and custom policies for your B2C application,
 * To learn more about user flows, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview
 * To learn more about custom policies, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview
 */
export const b2cPolicies = {
  productionLogOutRedirect: "https://portal.permarisk.gov",

  names: {
    editProfile: "B2C_1_edit_profile",
    Signin: "B2C_1_signin",
  },
  authorities: {
    editProfile: {
      authority:
        "https://permaportal.b2clogin.com/permaportal.onmicrosoft.com/B2C_1_edit_profile",
    },
    Signin: {
      authority:
        "https://permaportal.b2clogin.com/permaportal.onmicrosoft.com/B2C_1_signin",
    },
  },
  authorityDomain: "permaportal.b2clogin.com",
};

/**
 * Configuration object to be passed to MSAL instance on creation.
 * For a full list of MSAL.js configuration parameters, visit:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md
 */
export const msalConfig: Configuration = {
  auth: {
    clientId: "d6663d04-fac4-4db4-bd66-1f46daafabe4", // This is the ONLY mandatory field that you need to supply.
    authority: b2cPolicies.authorities.Signin.authority, // Defaults to "https://login.microsoftonline.com/common"
    knownAuthorities: [b2cPolicies.authorityDomain], // Mark your B2C tenant's domain as trusted.
    redirectUri: "/", // Points to window.location.origin. You must register this URI on Azure portal/App Registration.
    postLogoutRedirectUri: "/", // Indicates the page to navigate after logout.
    navigateToLoginRequestUrl: true, // If "true", will navigate back to the original request location before processing the auth code response.
  },
  cache: {
    cacheLocation: BrowserCacheLocation.LocalStorage, // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO between tabs.
    storeAuthStateInCookie: isIE, // Set this to "true" if you are having issues on IE11 or Edge
  },
  system: {
    loggerOptions: {
      loggerCallback(logLevel: LogLevel, message: string, containsPii) {
        console.log(message);
      },
      logLevel: LogLevel.Info,
      piiLoggingEnabled: false,
    },
  },
};
export const protectedResources = {
  applicationFileUploadServiceApi: {
    endpoint: `${environment.apiEndpoint}/api/ApplicationFileUpload`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  memberFileUploadServiceApi: {
    endpoint: `${environment.apiEndpoint}/api/MemberFileUpload`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  applicationSectionElementsApi: {
    endpoint: `${environment.apiEndpoint}/api/ApplicationSectionElements`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  applicationSectionsApi: {
    endpoint: `${environment.apiEndpoint}/api/ApplicationSections`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  membersApi: {
    endpoint: `${environment.apiEndpoint}/api/Members`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  linesApi: {
    endpoint: `${environment.apiEndpoint}/api/Lines`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  categoriesApi: {
    endpoint: `${environment.apiEndpoint}/api/Categories`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  contactsApi: {
    endpoint: `${environment.apiEndpoint}/api/Contacts`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  applicationsApi: {
    endpoint: `${environment.apiEndpoint}/api/Applications`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
  applicationTypesApi: {
    endpoint: `${environment.apiEndpoint}/api/ApplicationTypes`,
    scopes: [
      "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
    ],
  },
};
/**
 * An optional silentRequest object can be used to achieve silent SSO
 * between applications by providing a "login_hint" property.
 */
export const silentRequest = {
  scopes: [
    "openid",
    "profile",
    "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
  ],
  loginHint: "example@domain.net",
};
