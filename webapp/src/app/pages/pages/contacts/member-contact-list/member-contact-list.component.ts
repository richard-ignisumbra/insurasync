import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import {
  Component,
  Directive,
  ElementRef,
  EventEmitter,
  OnInit,
  Output,
  TemplateRef,
  ViewChild,
} from "@angular/core";
import { Contact, MemberDetails } from "src/app/core/api/v1";

import { AddContactComponent } from "../../../../../app/pages/pages/contacts/add-contact/add-contact.component";
import { AddExistingContactComponent } from "../add-existing-contact/add-existing-contact.component";
import { HttpClient } from "@angular/common/http";
import { MatDialog } from "@angular/material/dialog";
import { MembersService } from "../../../../core/api/v1//api/members.service";
import { PopoverService } from "../../../../../@vex/components/popover/popover.service";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icPersonAdd from "@iconify/icons-ic/twotone-person-add";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-member-contact-list",
  templateUrl: "./member-contact-list.component.html",
  styleUrls: ["./member-contact-list.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class MemberContactListComponent implements OnInit {
  membersEndpoint: string = protectedResources.membersApi.endpoint;
  private _MemberDetails = new BehaviorSubject<MemberDetails>(null);
  MemberDetails$ = this._MemberDetails.asObservable();

  private _contactList = new BehaviorSubject<Contact[]>(null);
  readonly ContactList$ = this._contactList.asObservable();
  icMoreVert = icMoreVert;
  ColumnMode = ColumnMode;
  memberId: number;
  hideAddContactForm: boolean = false;
  @ViewChild("addContactTemplate", { read: TemplateRef })
  addContactTemplate: TemplateRef<any>;
  createNewContactDialog: any;
  addExistingContactDialog: any;
  private parametersSubscription: any;
  icPersonAdd = icPersonAdd;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private popoverService: PopoverService,
    private dialog: MatDialog,
    private memberService: MembersService
  ) {}
  @Output()
  selectContact: EventEmitter<number> = new EventEmitter<number>();

  ngOnInit(): void {
    this.parametersSubscription = this.route.paramMap.subscribe(
      (params: ParamMap) => {
        let paramID = params.get("id");
        if (paramID) {
          this.memberId = +paramID;
          this.getContacts();
          this.getMemberDetails(this.memberId);
        }
      }
    );
  }

  public getContacts() {
    this.memberService
      .apiMembersIdContactsGet(this.memberId)
      .subscribe((contacts) => {
        this._contactList.next(contacts);
      });
  }

  getMemberDetails(id: any) {
    this.http
      .get(this.membersEndpoint + "/" + id)
      .subscribe((returnMembers) => {
        console.log("success!");
        console.log(returnMembers);
        this._MemberDetails.next(returnMembers);
      });
  }
  viewDetails(row: any) {
    console.log("viewDetails Contacts", row);

    console.log("emitting - " + row.contactID);
    this.selectContact.emit(row.contactID);
  }
  remove(id: any) {
    if (confirm("Are you sure you want to remove this contact")) {
      this.memberService
        .apiMembersIdContactsContactIdDelete(this.memberId, id)
        .subscribe((e) => {
          if (e) {
            this.getContacts();
          }
        });
    }
  }
  makeprimary(id: any) {
    if (confirm("Are you sure you want to make this contact primary")) {
      this.memberService
        .apiMembersIdContactsContactIdMakePrimaryPost(this.memberId, id)
        .subscribe((e) => {
          if (e) {
            this.getContacts();
          }
        });
    }
  }
  showAddContact(origin: ElementRef | HTMLElement) {
    let theDialog = this.dialog.open(AddContactComponent, {
      data: this.memberId,
      width: "600px",
    });

    theDialog.afterClosed().subscribe((e) => {
      if (e && e.action && e.action == "addNewContact") {
        this.showAddContact(origin);
      } else {
        this.getContacts();
      }
    });
  }
  showAddExistingContact(origin: ElementRef | HTMLElement) {
    let theDialog = this.dialog.open(AddExistingContactComponent, {
      data: this.memberId,
      width: "600px",
    });

    theDialog.afterClosed().subscribe((e) => {
      if (e && e.action && e.action == "addNewContact") {
        this.showAddContact(origin);
      } else {
        this.getContacts();
      }
    });
  }
}
