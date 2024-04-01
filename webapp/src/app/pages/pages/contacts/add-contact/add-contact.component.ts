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
  MatDialog,
  MAT_DIALOG_DATA,
  MatDialogRef,
} from "@angular/material/dialog";

import { HttpClient } from "@angular/common/http";
import { MatSnackBar } from "@angular/material/snack-bar";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSmartphone from "@iconify/icons-ic/twotone-smartphone";
import icweb from "@iconify/icons-ic/web";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-add-contact",
  templateUrl: "./add-contact.component.html",
  styleUrls: ["./add-contact.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class AddContactComponent implements OnInit {
  contactsEndpoint: string = protectedResources.contactsApi.endpoint;

  ColumnMode = ColumnMode;

  isSaving: boolean = false;
  icMoreVert = icMoreVert;
  icSmartphone = icSmartphone;
  icweb = icweb;
  ContactDetails: any = {};
  private _memberId: number;

  createContactForm = new FormGroup({
    name: new FormControl(""),
    email: new FormControl(""),
    password: new FormControl(""),
    confirmPassword: new FormControl(""),
  });

  private parametersSubscription: any;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<AddContactComponent>,
    @Inject(MAT_DIALOG_DATA) private memberId: string
  ) {}

  ngOnInit(): void {}

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
    //    this.dialogRef.close(true);
  }
  public validateContact(): boolean {
    let result = false;
    if (
      this.ContactDetails.firstName &&
      this.ContactDetails.lastName &&
      this.ContactDetails.email &&
      this.ContactDetails.contactType
    ) {
      result = true;
    }
    return result;
  }
  public createContact() {
    this.ContactDetails.InitialMemberID = Number(this.memberId);
    this.http
      .post(this.contactsEndpoint, this.ContactDetails)
      .subscribe((returnMembers) => {
        console.log("success saving contact");
        console.log(returnMembers);
        this.closeModal();
        this.snackBar.open("Contact Created Successfully", "CLOSE", {
          duration: 3000,
          horizontalPosition: "right",
        });
      });
  }
  public saveContact() {
    if (this.validateContact()) {
      console.log("valid create contact");
      this.isSaving = true;
      this.createContact();
    } else {
      console.log("error not valid");
    }
  }
}
