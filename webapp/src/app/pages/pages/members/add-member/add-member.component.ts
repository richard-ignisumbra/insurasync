import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { ChangeDetectorRef, Component, OnInit, ViewChild } from "@angular/core";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import {
  FormBuilder,
  NgForm,
  ValidationErrors,
  Validators,
} from "@angular/forms";

import { FormGroup } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MemberDetails } from "../../../../core/api/v1/model/memberDetails";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSmartphone from "@iconify/icons-ic/twotone-smartphone";
import icweb from "@iconify/icons-ic/web";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-add-member",
  templateUrl: "./add-member.component.html",
  styleUrls: ["./add-member.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class AddMemberComponent implements OnInit {
  membersEndpoint: string = protectedResources.membersApi.endpoint;
  form: FormGroup;
  availableMembers: any[];
  ColumnMode = ColumnMode;
  memberId: number;
  MemberDetails: MemberDetails = {};
  isSaving: boolean = false;
  icMoreVert = icMoreVert;
  icSmartphone = icSmartphone;
  icweb = icweb;
  public noParentMember: boolean = true;
  private parametersSubscription: any;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private _snackBar: MatSnackBar,
    private ref: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getMembers();
    this.form = this.fb.group({
      memberName: [this.MemberDetails.memberName, [Validators.required]],
      memberNumber: [this.MemberDetails.memberNumber, [Validators.required]],
      parentMemberId: [this.memberId],
      organizationType: [
        this.MemberDetails.organizationType,
        [Validators.required],
      ],
      noParentMember: [this.noParentMember],
    });
    this.form.controls["parentMemberId"].valueChanges.subscribe((value) => {
      if (value != null) {
        console.log("test");
        this.noParentMember = false;
        this.ref.detectChanges();
      }
    });
  }

  getMembers() {
    console.log(this.membersEndpoint);

    this.http.get(this.membersEndpoint).subscribe((returnMembers) => {
      console.log("success!");
      console.log(returnMembers);
      let filteredMembers = returnMembers as any[];
      console.log("filtered members");
      console.log(filteredMembers);
      this.availableMembers = filteredMembers.filter(
        (member) => member.parentMember === null
      );
    });
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
  updateParentMember(option, event) {
    console.log("updateParentMember");
    if (event.checked === true) {
      this.MemberDetails.parentMember = null;
      this.form.controls["parentMemberId"].setValue(null);
    }
  }
  public parentComparisonFunction(option, value): boolean {
    if (value && option) {
      return option?.memberId === value?.parentMember?.memberId;
    }
  }

  public saveMember() {
    if (
      this.form.valid == false ||
      (this.noParentMember == false &&
        this.form.controls["parentMemberId"].value == null)
    ) {
      let totalErrors = 0;
      Object.keys(this.form.controls).forEach((key) => {
        const controlErrors: ValidationErrors = this.form.get(key).errors;
        if (controlErrors != null) {
          this.form.controls[key].markAsTouched();
          totalErrors++;
          Object.keys(controlErrors).forEach((keyError) => {
            console.log(
              "Key control: " +
                key +
                ", keyError: " +
                keyError +
                ", err value: ",
              controlErrors[keyError]
            );
          });
        }
      });
      this.openSnackBar("Please correct the errors on the form", "OK");
    } else {
      console.log("saveMember");
      this.isSaving = true;
      this.MemberDetails.memberStatus = "Active";
      this.MemberDetails.memberName = this.form.controls["memberName"].value;
      this.MemberDetails.memberNumber =
        this.form.controls["memberNumber"].value;
      this.MemberDetails.organizationType =
        this.form.controls["organizationType"].value;
      this.MemberDetails.parentMember =
        this.form.controls["parentMemberId"].value;

      this.http
        .post(this.membersEndpoint, this.MemberDetails)

        .subscribe((returnMemberId) => {
          console.log("success");

          console.log(returnMemberId);
          this.router.navigate([`members/details/${returnMemberId}`]);
        });
    }
  }
  public openSnackBar(
    message: string,
    action: string,
    duration: number = 3000
  ) {
    this._snackBar.open(message, action, { duration: duration });
  }
}
