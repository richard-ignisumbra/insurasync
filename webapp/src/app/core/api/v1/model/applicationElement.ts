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
import { ElementType } from './elementType';
import { ListValue } from './listValue';
import { ApplicationElementResponse } from './applicationElementResponse';


export interface ApplicationElement { 
    elementId?: number;
    longText?: string | null;
    shortName?: string | null;
    elementType?: ElementType;
    tableSectionId?: number | null;
    decimalPrecision?: number | null;
    isRequired?: boolean;
    maxLength?: number | null;
    indentSpaces?: number | null;
    allowNewRows?: boolean;
    readonly?: boolean;
    width?: number | null;
    label?: string | null;
    sumValues?: boolean;
    selectOptions?: Array<ListValue> | null;
    responses?: Array<ApplicationElementResponse> | null;
    previousResponses?: Array<ApplicationElementResponse> | null;
}

