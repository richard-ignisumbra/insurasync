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
import { Contact } from './contact';


export interface Member { 
    memberId?: number;
    memberName?: string | null;
    memberNumber?: string | null;
    memberStatus?: string | null;
    organizationType?: string | null;
    primaryContact?: Contact;
    parentMember?: Member;
}

