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
import { Component, Injector, Input, OnInit, Inject } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import {
  MatDialog,
  MAT_DIALOG_DATA,
  MatDialogRef,
} from "@angular/material/dialog";
import { Attachment, AttachmentCategory } from "src/app/core/api/v1";
import { HttpClient } from "@angular/common/http";
import { MembersService } from "../../../../core/api/v1/api/members.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSmartphone from "@iconify/icons-ic/twotone-smartphone";
import icweb from "@iconify/icons-ic/web";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";
import { CategoriesService } from "src/app/core/api/v1/api/categories.service";
interface attachmentInformation {
  memberId: number;
  attachmentId?: number;
}
interface attachmentDetails {
  API: string;

  is_multiple_selection_allowed: boolean;

  memberId: number;
  policyPeriod: number;
  status: string;
  category: AttachmentCategory;
  description: string;
}
@Component({
  selector: "vex-add-attachment",
  templateUrl: "./add-attachment.component.html",
  styleUrls: ["./add-attachment.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class AddAttachmentComponent implements OnInit {
  ColumnMode = ColumnMode;
  private _AttachmentCategories = new BehaviorSubject<AttachmentCategory[]>(
    null
  );
  AttachmentCategories$ = this._AttachmentCategories.asObservable();
  isSaving: boolean = false;
  icMoreVert = icMoreVert;
  icSmartphone = icSmartphone;
  icweb = icweb;
  years: number[] = [];

  AttachmentCategory: AttachmentCategory = {
    categoryTitle: "General",
    categoryId: 1,
  };
  private _memberId: number;
  public file_upload_config: attachmentDetails = {
    API: "api/MemberFileUpload",

    is_multiple_selection_allowed: true,

    memberId: 0,
    policyPeriod: 0,
    status: "Active",
    category: { categoryId: 1, categoryTitle: "General" },
    description: "",
  };
  createNoteForm = new FormGroup({
    category: new FormControl(""),
    subject: new FormControl(""),
    content: new FormControl(""),
    status: new FormControl(""),
  });

  private parametersSubscription: any;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private memberService: MembersService,
    private dialogRef: MatDialogRef<AddAttachmentComponent>,
    @Inject(MAT_DIALOG_DATA) private attachmentInfo: attachmentInformation,
    private categoryService: CategoriesService
  ) {}

  ngOnInit(): void {
    this._memberId = this.attachmentInfo!.memberId;
    this.file_upload_config.memberId = this._memberId;
    this.categoryService
      .apiCategoriesAttachmentCategoriesGet()
      .subscribe((data) => {
        this._AttachmentCategories.next(data);
        this.file_upload_config.category = data[0];
        if (
          this.attachmentInfo!.attachmentId != null &&
          this.attachmentInfo!.attachmentId != 0
        ) {
          this.getNoteInfo(this.attachmentInfo!.attachmentId);
        }
      });
    let startYear = 2021;
    let endYear = new Date().getFullYear() + 1;
    for (let i = startYear; i <= endYear; i++) {
      this.years.push(i);
    }
    this.file_upload_config.policyPeriod = new Date().getFullYear();
  }
  getNoteInfo(noteId: number) {
    this.memberService
      .apiMembersMemberIdNotesNoteIdGet(this._memberId, noteId)
      .subscribe((note) => {
        // this.AttachmentDetails = note;
        this.AttachmentCategories$.subscribe((categories) => {
          this.AttachmentCategory = categories.find(
            (c) => c.categoryId == note.categoryId
          );
        });
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
  closeModal() {
    //   this.inj.get(INIT_DATA)._service.close();
    console.log("Close modal");

    this.dialogRef.close();
    //    this.dialogRef.close(true);
  }
  public validateNote(): boolean {
    let result = false;
    console.log(this.createNoteForm.controls["subject"].value);

    if (this.file_upload_config.description != "") {
      result = true;
    }
    return result;
  }
  public onCategoryChange(event) {
    console.log("onCategoryChange", this.AttachmentCategory);
    this.file_upload_config.category.categoryId =
      this.AttachmentCategory.categoryId;
    this.file_upload_config.category.categoryTitle =
      this.AttachmentCategory.categoryTitle;
  }

  public saveNote() {
    // this.memberService
    //   .apiMembersMemberIdNotesPost(
    //     Number(this._memberId),
    //     this.AttachmentDetails
    //   )
    //   .subscribe((noteId) => {
    //     console.log("success");
    //     this.closeModal();
    //   });
  }
  public deleteNote() {
    // this.AttachmentDetails.status = "deleted";
    this.isSaving = true;
    this.saveNote();
  }
  UploadSuccess(uploadResults: string) {
    console.log("UploadSuccess", uploadResults);
    this.closeModal();
  }
  public saveNoteForm() {
    if (this.validateNote()) {
      console.log("valid create contact");
      this.isSaving = true;
      this.saveNote();
    } else {
      console.log("error not valid");
    }
  }
}
