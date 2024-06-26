/**
 * permaAPI
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 *
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

export enum GroupType {
  None = "None",
  Annual = "Annual",
  Quarterly = "Quarterly",
  Monthly = "Monthly",
}

export interface ApplicationType {
  applicationTypeId?: number;
  type?: string | null;
  notifyEmails?: string | null;
  adminPermissionId?: number | null;
  reportType?: number | null;
  groupType: GroupType;
}
