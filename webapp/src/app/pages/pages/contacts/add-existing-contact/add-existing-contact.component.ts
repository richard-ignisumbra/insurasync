import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import { Component, Injector, Input, OnInit, Inject } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from "@angular/material/dialog";
import { map, startWith } from "rxjs/operators";

import { HttpClient } from "@angular/common/http";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Observable } from "rxjs";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSmartphone from "@iconify/icons-ic/twotone-smartphone";
import icweb from "@iconify/icons-ic/web";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-add-existing-contact",
  templateUrl: "./add-existing-contact.component.html",
  styleUrls: ["./add-existing-contact.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class AddExistingContactComponent implements OnInit {
  contactsEndpoint: string = protectedResources.contactsApi.endpoint;
  membersEndpoint: string = protectedResources.membersApi.endpoint;
  Contacts: any = [];
  existingContacts: any = [];
  ColumnMode = ColumnMode;
  contact = new FormControl();
  isSaving: boolean = false;
  icMoreVert = icMoreVert;
  icSmartphone = icSmartphone;
  icweb = icweb;
  ContactDetails: any = {};
  private _memberId: number;
  filteredOptions: Observable<string[]>;
  createContactForm = new FormGroup({
    name: new FormControl(""),
  });

  private parametersSubscription: any;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<AddExistingContactComponent>,
    @Inject(MAT_DIALOG_DATA) private memberId: string
  ) {}

  ngOnInit(): void {
    this.getMemberContacts();
    this.filteredOptions = this.contact.valueChanges.pipe(
      startWith(""),
      map((value) => this._filter(value))
    );
  }
  private _filter(value: any): string[] {
    let filterValue = "";
    if (value["displayName"]) {
      filterValue = value.displayName;
    } else {
      filterValue = value;
    }

    return this.Contacts.filter((option) =>
      option.displayName.toLowerCase().includes(filterValue)
    );
  }
  hasProperty(theList: string[], searchValue: string): boolean {
    let item = theList?.find((e) => e === searchValue);
    console.log("hasProperty", item);
    if (item) {
      return true;
    } else {
      return false;
    }
  }
  closeModal() {
    //   this.inj.get(INIT_DATA)._service.close();
    console.log("Close modal");
    this.dialogRef.close();
  }
  AddNewContact() {
    //   this.inj.get(INIT_DATA)._service.close();
    console.log("add Contact modal");
    this.dialogRef.close({ action: "addNewContact" });
  }
  displayContact(contact?: any): string {
    return contact?.displayName;
  }

  selectContact(contact: any) {
    console.log("selectContact");
  }

  public validateContact(): boolean {
    if (this.contact.value) {
      return true;
    } else {
      return false;
    }
  }

  public getMemberContacts() {
    this.http
      .get(this.membersEndpoint + "/" + this.memberId + "/contacts")
      .subscribe((memberContacts: any[]) => {
        console.log("member Contacts success!");
        console.log(memberContacts);
        this.existingContacts = memberContacts;
        this.getAvailableContacts();
      });
  }
  public getAvailableContacts() {
    this.http.get(this.contactsEndpoint).subscribe((contacts: any[]) => {
      console.log("available contacts success!");
      console.log(contacts);
      let map = new Map();
      this.existingContacts.forEach((memberContact) => {
        map.set(memberContact.contactID, true);
      });
      let filteredContacts = contacts.filter((i) => !map[i.contactID]);
      this.Contacts = filteredContacts;
    });
  }
  public addContacttoMember() {
    this.isSaving = false;
    let contactBody = { contactID: this.contact.value.contactID };
    this.http
      .post(
        this.membersEndpoint + "/" + this.memberId + "/contact",
        contactBody
      )
      .subscribe((result) => {
        console.log("Add Existing Contact success!");
        this.closeModal();
        this.snackBar.open("Contact Added Successfully", "CLOSE", {
          duration: 3000,
          horizontalPosition: "right",
        });
      });
  }
  public saveContact() {
    if (this.validateContact()) {
      console.log("valid create contact");
      console.log(this.contact.value);
      this.isSaving = true;
      this.addContacttoMember();
    } else {
      console.log("error not valid");
    }
  }
}
