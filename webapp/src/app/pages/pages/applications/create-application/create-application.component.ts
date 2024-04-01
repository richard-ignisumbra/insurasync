import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormControl, Validators } from "@angular/forms";
import { Observable, Subject, combineLatest } from "rxjs";
import { map, startWith, switchMap, takeUntil, tap } from "rxjs/operators";

import { ApplicationType } from "../../../../core/api/v1/model/applicationType";
import { ApplicationTypesService } from "../../../../core/api/v1/api/applicationTypes.service";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import { ColumnMode } from "@swimlane/ngx-datatable";
import { GroupType } from "../../../../core/api/v1/model/applicationType";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";
import { MatDialogRef } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Member } from "../../../../core/api/v1/model/member";
import { MembersService } from "src/app/core/api/v1";
import { NewApplication } from "../../../../core/api/v1/model/newApplication";
import { Router } from "@angular/router";
import icBusiness from "@iconify/icons-ic/twotone-business";
import icClose from "@iconify/icons-ic/twotone-close";
import icDelete from "@iconify/icons-ic/twotone-delete";
import icDownload from "@iconify/icons-ic/twotone-cloud-download";
import icEmail from "@iconify/icons-ic/twotone-mail";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icPerson from "@iconify/icons-ic/twotone-person";
import icPhone from "@iconify/icons-ic/twotone-phone";
import icPrint from "@iconify/icons-ic/twotone-print";
import icStar from "@iconify/icons-ic/twotone-star";

function memberSelectionValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const isValidMember =
      control.value &&
      typeof control.value === "object" &&
      "memberId" in control.value;
    return !isValidMember ? { invalidMember: { value: control.value } } : null;
  };
}
@Component({
  selector: "vex-create-application",
  templateUrl: "./create-application.component.html",
  styleUrls: ["./create-application.component.scss"],
})
export class CreateApplicationComponent implements OnInit {
  Applications: any = [];

  form = this.fb.group({
    applicationName: [null, [Validators.required]],

    applicationTypeId: [null, [Validators.required]],
    coverageYear: new Date().getFullYear(),
    dueDate: [null, [Validators.required]],
    periodQuarter: [null],
    periodMonth: [null],
  });
  ColumnMode = ColumnMode;
  icClose = icClose;
  icStar = icStar;
  icMoreVert = icMoreVert;
  displayQuarter$: Observable<boolean>;
  displayMonth$: Observable<boolean>;
  icPrint = icPrint;
  icDownload = icDownload;
  icDelete = icDelete;
  isSaving: boolean = false;
  icBusiness = icBusiness;
  icPerson = icPerson;
  icEmail = icEmail;
  icPhone = icPhone;
  members: Member[] = []; // Store all members here
  filteredMembers$: Observable<Member[]>; // Used for filtering and displaying in the autocomplete

  applicationTypes$: Observable<ApplicationType[]>;
  private destroy$ = new Subject<void>();
  memberInputControl = new FormControl("", [memberSelectionValidator()]);

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private applicationsService: ApplicationsService,
    private applicationTypesService: ApplicationTypesService,
    private memberService: MembersService,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<CreateApplicationComponent>
  ) {}
  ngOnInit(): void {
    this.applicationTypes$ =
      this.applicationTypesService.apiApplicationTypesGet();
    this.getMembers();
    this.setupVisibilityControls();
  }

  getMembers() {
    this.memberService.apiMembersGet().subscribe((members) => {
      this.members = members;
      this.setupFilter();
    });
  }
  setupFilter() {
    this.filteredMembers$ = this.memberInputControl.valueChanges.pipe(
      startWith(""),
      map((value) => (typeof value === "string" ? value : value.memberName)),
      map((name) => (name ? this.filterMembers(name) : this.members.slice()))
    );
  }

  filterMembers(val: string): Member[] {
    console.log("filterMembers", val);
    if (val) {
      return this.members.filter((member) =>
        member.memberName.toLowerCase().includes(val.toLowerCase())
      );
    } else {
      return this.members;
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  private setupVisibilityControls() {
    const applicationTypeChanges$ = this.form
      .get("applicationTypeId")
      .valueChanges.pipe(
        startWith(this.form.get("applicationTypeId").value),
        map((selectedId) => +selectedId), // Convert string value to number
        tap((selectedId) => console.log("Application Type ID:", selectedId)) // Log the current applicationTypeId
      );

    // Combine the latest values of applicationTypes$ and applicationTypeChanges$
    const selectedApplicationType$: Observable<ApplicationType> = combineLatest(
      [this.applicationTypes$, applicationTypeChanges$]
    ).pipe(
      tap(([types, selectedId]) =>
        console.log("Before finding the type:", types, selectedId)
      ),
      map(([types, selectedId]) =>
        types.find((type) => type.applicationTypeId === selectedId)
      ),
      tap((foundType) => console.log("Found Application Type:", foundType)) // Log the found application type
    );

    // Determine whether to display quarter or month fields
    this.displayQuarter$ = selectedApplicationType$.pipe(
      tap((type) => {
        if (type?.groupType === GroupType.Quarterly) {
          this.form.get("periodQuarter").setValidators([Validators.required]);
          this.form.get("periodQuarter").updateValueAndValidity();
          this.form.get("periodMonth").clearValidators();
          this.form.get("periodMonth").updateValueAndValidity();
        } else if (type?.groupType === GroupType.Monthly) {
          this.form.get("periodMonth").setValidators([Validators.required]);
          this.form.get("periodMonth").updateValueAndValidity();
          this.form.get("periodQuarter").clearValidators();
          this.form.get("periodQuarter").updateValueAndValidity();
        } else {
          this.form.get("periodQuarter").clearValidators();
          this.form.get("periodQuarter").updateValueAndValidity();
          this.form.get("periodMonth").clearValidators();
          this.form.get("periodMonth").updateValueAndValidity();
        }
      }),
      map((type) => type?.groupType === GroupType.Quarterly),
      tap((isQuarterly) => console.log("Is Quarterly:", isQuarterly)) // Log the result of the Quarterly check
    );

    this.displayMonth$ = selectedApplicationType$.pipe(
      tap((type) => {
        if (type?.groupType === GroupType.Quarterly) {
          this.form.get("periodQuarter").setValidators([Validators.required]);
          this.form.get("periodQuarter").updateValueAndValidity();
          this.form.get("periodMonth").clearValidators();
          this.form.get("periodMonth").updateValueAndValidity();
        } else if (type?.groupType === GroupType.Monthly) {
          this.form.get("periodMonth").setValidators([Validators.required]);
          this.form.get("periodMonth").updateValueAndValidity();
          this.form.get("periodQuarter").clearValidators();
          this.form.get("periodQuarter").updateValueAndValidity();
        } else {
          this.form.get("periodQuarter").clearValidators();
          this.form.get("periodQuarter").updateValueAndValidity();
          this.form.get("periodMonth").clearValidators();
          this.form.get("periodMonth").updateValueAndValidity();
        }
      }),
      map((type) => type?.groupType === GroupType.Monthly),
      tap((isMonthly) => console.log("Is Monthly:", isMonthly)) // Log the result of the Monthly check
    );
  }
  onMemberSelected(event: MatAutocompleteSelectedEvent): void {
    // Assuming you want to keep the selected member object in the form control
    // You might need to adjust based on your requirements
    this.memberInputControl.setValue(event.option.value);
  }

  getApplicationFromFrom(): NewApplication {
    let newApplication: NewApplication = {};
    console.log("getApplicationFromForm", this.memberInputControl.value);
    newApplication.applicationName = this.form.get("applicationName").value;
    newApplication.memberId = this.memberInputControl.value.memberId;
    newApplication.applicationTypeId = this.form.get("applicationTypeId").value;
    newApplication.coverageYear = this.form.get("coverageYear").value;
    newApplication.dueDate = this.form.get("dueDate").value;
    newApplication.applicationStatus = "New";
    newApplication.periodMonth = this.form.get("periodMonth").value;
    newApplication.periodQuarter = this.form.get("periodQuarter").value;
    return newApplication;
  }
  save() {
    console.log("save");
    if (this.form.invalid || this.memberInputControl.invalid) {
      // Trigger validation feedback to the user
      this.form.markAllAsTouched();
      console.log("form is invalid");
      // Loop through all form controls
      Object.keys(this.form.controls).forEach((key) => {
        const controlErrors = this.form.get(key).errors;
        if (controlErrors) {
          // If there are errors, log them to the console
          Object.keys(controlErrors).forEach((errorKey) => {
            console.log(
              `Control: ${key}, Error: ${errorKey}, Value:`,
              controlErrors[errorKey]
            );
          });
        }
      });
      return;
    }

    const app: NewApplication = this.getApplicationFromFrom();

    this.applicationsService.apiApplicationsPut(app).subscribe({
      next: (newApplicationId) => {
        console.log("Application created", newApplicationId);
        this.snackBar.open("Application Added Successfully", "CLOSE", {
          duration: 3000,
        });
        this.closeModal();
      },
      error: (error) => {
        // Handle any errors here, such as displaying a message to the user
        console.error("Error creating application", error);
        this.snackBar.open("Error adding application", "CLOSE", {
          duration: 3000,
        });
      },
    });
  }
  checkForSelection(): void {
    this.memberInputControl.markAsTouched();
    this.memberInputControl.updateValueAndValidity();
  }

  closeModal() {
    //   this.inj.get(INIT_DATA)._service.close();
    console.log("Close modal");

    this.dialogRef.close();
    //    this.dialogRef.close(true);
  }
  /* Handle form errors in Angular 8 */
  public errorHandling = (control: string, error: string) => {
    return this.form.controls[control].hasError(error);
  };
  displayMember(member: Member): string {
    // Check if member is valid and has memberName property
    return member && member.memberName ? member.memberName : "";
  }
}
