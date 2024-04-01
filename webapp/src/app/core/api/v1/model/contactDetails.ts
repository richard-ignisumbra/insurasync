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
import { Member } from './member';


export interface ContactDetails { 
    contactID?: number;
    firstName?: string | null;
    lastName?: string | null;
    readonly displayName?: string | null;
    email?: string | null;
    salutation?: string | null;
    middleName?: string | null;
    jobTitle?: string | null;
    contactType?: string | null;
    businessPhone?: string | null;
    homePhone?: string | null;
    mobilePhone?: string | null;
    fax?: string | null;
    addressType?: string | null;
    address1?: string | null;
    address2?: string | null;
    city?: string | null;
    state?: string | null;
    zip?: string | null;
    phone?: string | null;
    description?: string | null;
    isBoardMember?: boolean;
    isExecutiveCommitteeMember?: boolean;
    isApplicationUser?: boolean;
    userIdentifier?: string | null;
    isPermaAdmin?: boolean;
    members?: Array<Member> | null;
}

