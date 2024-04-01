import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from "@angular/core";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";

import { AppUtilityService } from "../../../../../app/shared-utilities/app-utility.service";
import { ContactsService } from "../../../../core/api/v1/api/contacts.service";
import { HttpClient } from "@angular/common/http";
import { MatSnackBar } from "@angular/material/snack-bar";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSmartphone from "@iconify/icons-ic/twotone-smartphone";
import icweb from "@iconify/icons-ic/web";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-member-contact-details",
  templateUrl: "./member-contact-details.component.html",
  styleUrls: ["./member-contact-details.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class MemberContactDetailsComponent implements OnInit {
  contactsEndpoint: string = protectedResources.contactsApi.endpoint;
  Contacts: any = [];
  icMoreVert = icMoreVert;
  ColumnMode = ColumnMode;
  newLoginResults: string = "";
  private _newLoginResults = new BehaviorSubject<string>("");
  newLoginResults$ = this._newLoginResults.asObservable();
  ContactDetails: any;
  @Output()
  ContactSaved: EventEmitter<boolean> = new EventEmitter<boolean>();

  private parametersSubscription: any;

  private _contactId: number;

  @Input() set ContactId(value: number) {
    this._contactId = value;
    this.GetContactDetails(value);
  }
  get ContactId(): number {
    return this._contactId;
  }

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    public appUtilityService: AppUtilityService,
    private contactsService: ContactsService,
    private ref: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.parametersSubscription = this.route.paramMap.subscribe(
      (params: ParamMap) => {
        let paramID = params.get("id");
        if (paramID) {
        }
      }
    );
    this.appUtilityService.UserProfile$.subscribe((data) => {});
  }
  GetContactDetails(contactId: number) {
    console.log("GetContactDetails");
    console.log(contactId);
    console.log(this.contactsEndpoint);

    this.http
      .get(this.contactsEndpoint + "/" + contactId)
      .subscribe((returnContact) => {
        console.log("success!");
        console.log(returnContact);
        this.ContactDetails = returnContact;
      });
  }
  public viewMemberDetails(member: any) {
    console.log("viewMemberDetails");
    this.router.navigateByUrl(
      "/members/details/" +
        member.memberId +
        "?fromContact=" +
        this.ContactDetails!.contactID
    );
  }
  public InviteUser() {
    if (confirm("do you really want to invite this user to the portal?")) {
      this.contactsService
        .apiContactsApiContactsContactIdInvitePut(this.ContactId)
        .subscribe((data) => {
          if (data.isSuccessful) {
            let userResult =
              "User has been invited to the portal.  Their login is " +
              this.ContactDetails.email +
              ", the  password is " +
              data.newPassword;
            if (data.emailResult != null && data.emailResult != "") {
              userResult +=
                ", invite email failed with status code - " + data.emailResult;
            }
            if (data.emailResult == null || data.emailResult == "") {
              userResult +=
                ", invite was emailed to user - " + this.ContactDetails.email;
            }
            this._newLoginResults.next(userResult);
            this.ContactDetails.isApplicationUser = true;
            confirm(userResult);
            this.ref.detectChanges();
          } else {
            confirm("User was not invited - " + data.inviteResult);
          }
        });
    }
  }
  public Makeadmin() {
    if (confirm("do you really wnat to make this user an admin?")) {
      if (
        confirm(
          "please confirm before allowing this user to have access to all data"
        )
      ) {
        this.contactsService
          .apiContactsApiContactsContactIdMakeadminPut(this.ContactId)
          .subscribe((data) => {
            if (data == true) {
              this.ContactDetails!.isPermaAdmin = true;
            } else {
              alert("an error occurred");
            }
          });
      }
    }
  }
  SaveContact() {
    console.log("saveMember");

    this.http
      .put(this.contactsEndpoint, this.ContactDetails)
      .subscribe((returnMembers) => {
        console.log("success saving contact");
        console.log(returnMembers);
        this.ContactSaved.emit(true);
        this.snackBar.open("Contact Saved Successfully", "CLOSE", {
          duration: 3000,
          horizontalPosition: "right",
        });
      });
  }
}
