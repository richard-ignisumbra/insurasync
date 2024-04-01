import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';

import { ApplicationFileUploadService } from './api/applicationFileUpload.service';
import { ApplicationSectionElementsService } from './api/applicationSectionElements.service';
import { ApplicationSectionsService } from './api/applicationSections.service';
import { ApplicationTypesService } from './api/applicationTypes.service';
import { ApplicationsService } from './api/applications.service';
import { CategoriesService } from './api/categories.service';
import { ContactsService } from './api/contacts.service';
import { LinesService } from './api/lines.service';
import { MemberFileUploadService } from './api/memberFileUpload.service';
import { MembersService } from './api/members.service';

@NgModule({
  imports:      [],
  declarations: [],
  exports:      [],
  providers: []
})
export class ApiModule {
    public static forRoot(configurationFactory: () => Configuration): ModuleWithProviders<ApiModule> {
        return {
            ngModule: ApiModule,
            providers: [ { provide: Configuration, useFactory: configurationFactory } ]
        };
    }

    constructor( @Optional() @SkipSelf() parentModule: ApiModule,
                 @Optional() http: HttpClient) {
        if (parentModule) {
            throw new Error('ApiModule is already loaded. Import in your base AppModule only.');
        }
        if (!http) {
            throw new Error('You need to import the HttpClientModule in your AppModule! \n' +
            'See also https://github.com/angular/angular/issues/20575');
        }
    }
}
