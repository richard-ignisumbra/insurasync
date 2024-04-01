import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from "@angular/common/http";

// auth.interceptor.ts
import { Injectable } from "@angular/core";
import { MsalService } from "@azure/msal-angular";
import { Observable } from "rxjs";
import { mergeMap } from "rxjs/operators";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: MsalService) {}

  protectedResources = {
    applicationFileUploadServiceApi: {
      endpoint: `/api/ApplicationFileUpload`,
      scopes: [],
    },
    memberFileUploadServiceApi: {
      endpoint: `/api/MemberFileUpload`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
    applicationSectionElementsApi: {
      endpoint: `/api/ApplicationSectionElements`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
    applicationSectionsApi: {
      endpoint: `/api/ApplicationSections`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
    membersApi: {
      endpoint: `/api/Members`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
    linesApi: {
      endpoint: `/api/Lines`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
    categoriesApi: {
      endpoint: `/api/Categories`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
    contactsApi: {
      endpoint: `/api/Contacts`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
    applicationsApi: {
      endpoint: `/api/Applications`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
    applicationTypesApi: {
      endpoint: `/api/ApplicationTypes`,
      scopes: [
        "https://permaportal.onmicrosoft.com/questionnaire-api/access_as_perma_user",
      ],
    },
  };

  getScopesForEndpoint(apiUrl: string): string[] {
    // Extract the base path (first two segments after the host) from the apiUrl
    const urlPath = new URL(apiUrl, "https://baseurl.com").pathname; // Dummy base URL for URL parsing

    const pathSegments = urlPath.split("/").filter((segment) => segment); // Remove empty segments
    console.log(pathSegments);
    if (pathSegments.length < 2) {
      console.error(
        "URL does not have enough segments for scope matching:",
        apiUrl
      );
      return []; // Return empty if not enough segments
    }
    const basePath = `/${pathSegments[0]}/${pathSegments[1]}`; // Construct the base path

    // Iterate through the entries of protectedResources to find a matching base path
    for (const [key, value] of Object.entries(this.protectedResources)) {
      if (value.endpoint.toLowerCase().startsWith(basePath.toLowerCase())) {
        // Return the scopes if the base path matches
        return value.scopes || [];
      }
    }

    // Log and return empty array if no match is found
    console.log("No matching scopes found for:", apiUrl);
    return [];
  }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // Attempt to fetch the token from MSAL

    return this.authService
      .acquireTokenSilent({
        scopes: this.getScopesForEndpoint(request.url),
      })
      .pipe(
        mergeMap((tokenResponse) => {
          console.log("token response", tokenResponse);
          const token = tokenResponse.accessToken;
          // Clone the request to add the new header.
          const authReq = request.clone({
            setHeaders: { Authorization: `Bearer ${token}` },
          });
          // Pass on the cloned request instead of the original request.
          return next.handle(authReq);
        })
      );
  }
}
