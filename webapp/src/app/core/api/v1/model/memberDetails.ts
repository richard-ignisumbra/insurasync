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
import { Contact } from './contact';


export interface MemberDetails { 
    memberId?: number;
    memberName?: string | null;
    memberNumber?: string | null;
    memberStatus?: string | null;
    organizationType?: string | null;
    primaryContact?: Contact;
    companyName?: string | null;
    address1?: string | null;
    address2?: string | null;
    city?: string | null;
    state?: string | null;
    zip?: string | null;
    phone?: string | null;
    website?: string | null;
    parentMember?: Member;
    startDate?: string | null;
    stateofIncorporation?: string | null;
    fein?: string | null;
    linesofCoverage?: Array<string> | null;
    notes?: string | null;
    joinDate?: string | null;
}

