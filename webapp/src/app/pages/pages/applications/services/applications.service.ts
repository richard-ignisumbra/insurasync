import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";

import { ApplicationPreview } from "../../../../core/api/v1/model/applicationPreview";
import { ApplicationReportType } from "../../../../core/api/v1/model/applicationReportType";
import { ApplicationType } from "../../../../core/api/v1/model/applicationType";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class ApplicationsServiceV2 {
  constructor(private http: HttpClient) {}

  baseURL() {
    return environment.apiEndpoint;
  }

  getApplicationTypes(): Observable<ApplicationType[]> {
    const url = `${this.baseURL()}/api/ApplicationTypes`;
    return this.http.get<ApplicationType[]>(url);
  }
  getLineofCoverages(): Observable<string[]> {
    const url = `${this.baseURL()}/api/Lines`;
    return this.http.get<string[]>(url);
  }
  getApplicationReportTypes(): Observable<ApplicationReportType[]> {
    const url = `${this.baseURL()}/api/ApplicationTypes/reportTypes`;
    return this.http.get<ApplicationReportType[]>(url);
  }
  getFilteredApplications(
    coverageYear?: number,
    status?: string,
    applicationType?: number,
    filterQuarter?: number,
    filterMonth?: number,
    lineofCoverage: string = "all"
  ): Observable<ApplicationPreview[]> {
    const url = `${this.baseURL()}/api/Applications/filtered`;
    // Initialize HttpParams
    let params = new HttpParams();

    // Conditionally append each parameter to the HttpParams object
    if (coverageYear !== undefined) {
      params = params.append("coverageYear", coverageYear.toString());
    }
    if (lineofCoverage !== undefined) {
      params = params.append("lineofCoverage", lineofCoverage.toString());
    }
    if (status !== undefined) {
      params = params.append("status", status);
    }
    if (applicationType !== undefined) {
      params = params.append("applicationType", applicationType.toString());
    }
    if (filterQuarter !== undefined && filterQuarter !== null) {
      params = params.append("filterQuarter", filterQuarter.toString());
    }
    if (filterMonth !== undefined && filterMonth !== null) {
      params = params.append("filterMonth", filterMonth.toString());
    }

    // Pass the HttpParams object to the GET request
    return this.http.get<ApplicationPreview[]>(url, { params });
  }
  ExportApplicationReport(
    coverageYear: number,
    reportId?: number,
    status?: string,
    filterQuarter?: number,
    filterMonth?: number,
    lineofCoverage: string = "all",
    applicationType?: number
  ): Observable<Blob> {
    const url = `${this.baseURL()}/api/Applications/${coverageYear}/export`;
    const body = {
      reportId: reportId,
      status,
      filterQuarter,
      filterMonth,
      lineofCoverage,
      applicationType,
    };
    return this.http.post(url, body, {
      responseType: "blob",
      headers: new HttpHeaders({
        Accept:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "Content-Type": "application/json",
      }),
    }) as Observable<Blob>; // Explicit cast here
  }

  exportApplications(
    coverageYear: number,
    reportTypeId: number,
    status: string = "all",
    filterQuarter?: number,
    filterMonth?: number,
    lineofCoverage: string = "all",
    applicationType?: number
  ): Observable<Blob> {
    const url = `${this.baseURL()}/api/Applications/${coverageYear}/Defaultexport`;
    const body = {
      reportTypeId: reportTypeId,
      status,
      filterQuarter,
      filterMonth,
      lineofCoverage,
      applicationType,
    };
    return this.http.post(url, body, {
      responseType: "blob",
      headers: new HttpHeaders({
        Accept:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "Content-Type": "application/json",
      }),
    }) as Observable<Blob>; // Explicit cast here
  }
}
