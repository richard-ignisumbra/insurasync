import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { Component, OnInit, ViewChild } from "@angular/core";
import { MatTab, MatTabGroup } from "@angular/material/tabs";

import { AppUtilityService } from "../../../../../app/shared-utilities/app-utility.service";
import { MemberContactListComponent } from "../../contacts/member-contact-list/member-contact-list.component";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";

@Component({
  selector: "app-member-tabbed",
  templateUrl: "./member-tabbed.component.html",
  styleUrls: ["./member-tabbed.component.scss"],
})
export class MemberTabbedComponent implements OnInit {
  icMoreVert = icMoreVert;
  showContactDetails: boolean = false;
  private parametersSubscription: any;
  id: string;
  tabIndex = 0;
  @ViewChild("t") tabGroup: MatTabGroup;
  selectedContact: number;
  @ViewChild("contactlist") memberContactList: MemberContactListComponent;
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    public appUtilityService: AppUtilityService
  ) {}
  onContactSelection(contactId: number) {
    console.log("onContactSelection");
    this.router.navigate([`members/details/${this.id}`], {
      queryParams: { contactId: contactId },
    });
    setTimeout(() => {
      this.tabGroup.selectedIndex = 2;
      console.log("changing tab", this.tabGroup.selectedIndex);
    });
  }
  onContactSaved(savedResult: boolean) {
    console.log("tabbed parent - onContactSaved");
    this.showContactDetails = false;
    this.tabIndex = 1;
    this.memberContactList.getContacts();
  }
  selectedIndexChange(event: any) {
    console.log(event);
  }
  ngOnInit() {
    this.parametersSubscription = this.route.paramMap.subscribe(
      (params: ParamMap) => {
        let paramID = params.get("id");
        if (paramID) {
          this.id = paramID;
        }
      }
    );
    this.route.queryParams.subscribe((params) => {
      if (params.contactId) {
        console.log(params.contactId);

        this.showContactDetails = true;
        this.selectedContact = params.contactId;
      }
    });
  }
  ngAfterViewInit() {
    if (this.showContactDetails) {
      setTimeout(() => {
        this.tabGroup.selectedIndex = 2;
        console.log("changing tab", this.tabGroup.selectedIndex);
      });
    }
    // this returns null
  }
}
