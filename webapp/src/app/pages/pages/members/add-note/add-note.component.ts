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
import { Note, NoteCategory } from "src/app/core/api/v1";
import { HttpClient } from "@angular/common/http";
import { MembersService } from "../../../../core/api/v1/api/members.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSmartphone from "@iconify/icons-ic/twotone-smartphone";
import icweb from "@iconify/icons-ic/web";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";
import { NoteCategoriesService } from "src/app/core/api/v1/api/noteCategories.service";
interface noteInformation {
  memberId: number;
  noteId?: number;
}
@Component({
  selector: "vex-add-note",
  templateUrl: "./add-note.component.html",
  styleUrls: ["./add-note.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class AddNoteComponent implements OnInit {
  ColumnMode = ColumnMode;
  private _NoteCategories = new BehaviorSubject<NoteCategory[]>(null);
  NoteCategories$ = this._NoteCategories.asObservable();
  isSaving: boolean = false;
  icMoreVert = icMoreVert;
  icSmartphone = icSmartphone;
  icweb = icweb;
  NoteDetails: Note = { status: "Active" };

  NoteCategory: NoteCategory = { categoryTitle: "General", categoryId: 1 };
  private _memberId: number;

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
    private dialogRef: MatDialogRef<AddNoteComponent>,
    @Inject(MAT_DIALOG_DATA) private noteInfo: noteInformation,
    private noteCategoryService: NoteCategoriesService
  ) {}

  ngOnInit(): void {
    this._memberId = this.noteInfo!.memberId;

    this.noteCategoryService.apiCategoriesGet().subscribe((data) => {
      this._NoteCategories.next(data);
      this.NoteCategory = data[0];
      if (this.noteInfo!.noteId != null && this.noteInfo!.noteId != 0) {
        this.getNoteInfo(this.noteInfo!.noteId);
      }
    });
  }
  getNoteInfo(noteId: number) {
    this.memberService
      .apiMembersMemberIdNotesNoteIdGet(this._memberId, noteId)
      .subscribe((note) => {
        this.NoteDetails = note;
        this.NoteCategories$.subscribe((categories) => {
          this.NoteCategory = categories.find(
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

    if (this.NoteDetails.content != "" && this.NoteDetails.subject != "") {
      result = true;
      this.NoteDetails.categoryId = this.NoteCategory.categoryId;
      this.NoteDetails.categoryTitle = this.NoteCategory.categoryTitle;
    }
    return result;
  }
  public onCategoryChange(event) {
    console.log("onCategoryChange", this.NoteCategory);
    this.NoteDetails.categoryId = this.NoteCategory.categoryId;
    this.NoteDetails.categoryTitle = this.NoteCategory.categoryTitle;
  }

  public saveNote() {
    this.memberService
      .apiMembersMemberIdNotesPost(Number(this._memberId), this.NoteDetails)
      .subscribe((noteId) => {
        console.log("success");
        this.closeModal();
      });
  }
  public deleteNote() {
    this.NoteDetails.status = "deleted";
    this.isSaving = true;
    this.saveNote();
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
