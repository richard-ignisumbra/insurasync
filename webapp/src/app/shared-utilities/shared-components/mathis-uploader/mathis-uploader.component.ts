import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

import { AppUtilityService } from "../../app-utility.service";
import { ApplicationFileUploadService } from "../../../core/api/v1/api/applicationFileUpload.service";
import { Subscription } from "rxjs";

@Component({
  selector: "app-mathis-file-uploader",
  templateUrl: "./mathis-uploader.component.html",
  styleUrls: ["./mathis-uploader.component.scss"],
})
export class MathisFileUploaderComponent implements OnInit, OnDestroy {
  @Input() config!: {
    API: string;
    MIME_types_accepted: string;
    is_multiple_selection_allowed: boolean;
    data: any;
    elementId: number;
    rowId: number;
    applicationId: number;
  };

  selected_files: {
    file: any;
    is_upload_in_progress: boolean;
    upload_result: any;
  }[] = [];
  @Output() UploadSuccess = new EventEmitter<string>();
  @ViewChild("fileSelector", { static: false }) file_selector!: ElementRef;

  file_selection_form: FormGroup;

  // Subscriptions
  private file_selection_sub!: Subscription;
  private file_upload_sub!: Subscription;

  constructor(
    private applicationFileUploadService: ApplicationFileUploadService,
    private global_utilities: AppUtilityService
  ) {
    this.file_selection_form = new FormGroup({
      file_selection: new FormControl(),
    });
  }

  ngOnInit(): void {
    this.trackFileSelection();
  }

  openFileSelector() {
    console.log(`Opening file selector for ${this.config.elementId}`);
    const file_selection = this.file_selector.nativeElement;
    file_selection.click();
  }

  trackFileSelection() {
    this.file_selection_sub = this.file_selection_form
      .get("file_selection")
      ?.valueChanges.subscribe(() => {
        const file_selection = this.file_selector.nativeElement;
        this.selectFiles(file_selection.files);
        this.file_selector.nativeElement.value = "";
      }) as Subscription;
  }

  selectFiles(incoming_files: any[]) {
    let incoming_file_count = incoming_files.length;
    let incorrect_MIME_type = false;
    for (let i = 0; i < incoming_file_count; i++) {
      let incoming_file = incoming_files[i];
      // Checking if MIME type is acceptable
      if (
        !!!this.config.MIME_types_accepted ||
        this.config.MIME_types_accepted.indexOf(incoming_file.type) >= 0
      ) {
        let selected_file = {
          file: incoming_file,
          is_upload_in_progress: false,
          upload_result: null,
        };
        this.selected_files.push(selected_file);
      } else {
        incorrect_MIME_type = true;
      }
    }
    // Display error
    if (incorrect_MIME_type) {
      let message =
        "Only files of the following MIME types are allowed: " +
        this.config.MIME_types_accepted;
      this.global_utilities.showSnackbar(message);
    }
  }
  availableFiles(): boolean {
    let available = false;
    for (const file of this.selected_files) {
      if (file.upload_result != "success") {
        available = true;
      }
    }

    return available;
  }
  uploadFile(index: number, uploadAll: boolean) {
    let file_for_upload = this.selected_files[index];
    const form_data = [];
    if (uploadAll) {
      let selected_file_count = this.selected_files.length;
      for (let i = 0; i < selected_file_count; i++) {
        let selected_file = this.selected_files[i];
        // Checking if the file can be uploaded
        if (
          !selected_file.is_upload_in_progress &&
          selected_file.upload_result != "success"
        ) {
          form_data.push(selected_file.file);
        }
      }
    } else {
      file_for_upload.is_upload_in_progress = true;
      file_for_upload.upload_result = null;
      form_data.push(file_for_upload.file);
    }

    console.log(`Uploading file`, this.config);
    console.log(form_data);
    this.applicationFileUploadService
      .apiApplicationFileUploadApplicationIdElementIdRowIdPost(
        this.config.applicationId,
        this.config.rowId,
        this.config.elementId,
        form_data
      )
      .subscribe(
        (response) => {
          // Adding dummy error
          if (uploadAll) {
            for (const file of this.selected_files) {
              file.is_upload_in_progress = false;
              if (file.file.name.indexOf("error") >= 0) {
                file.upload_result =
                  this.global_utilities.error_messages.service_failure;
              } else {
                file.upload_result = "success";
              }
            }
          } else {
            if (file_for_upload.file.name.indexOf("error") >= 0) {
              file_for_upload.upload_result =
                this.global_utilities.error_messages.service_failure;
            }

            file_for_upload.is_upload_in_progress = false;
          }
          this.UploadSuccess.emit(response);
        },
        (error) => {
          if (uploadAll) {
            for (const file of this.selected_files) {
              file.is_upload_in_progress = false;
              file.upload_result = error.message;
            }
          } else {
            file_for_upload.upload_result = error.message;
            file_for_upload.is_upload_in_progress = false;
          }
        }
      );
  }

  uploadAll() {
    this.uploadFile(0, true);
  }

  inititateFileCancel(index: number) {
    let file_for_upload = this.selected_files[index];
    if (file_for_upload.is_upload_in_progress) {
      this.displayFileUploadAbortConfirmation(() => {
        this.cancelFile(index);
      });
    } else {
      this.cancelFile(index);
    }
  }

  displayFileUploadAbortConfirmation(cancel_method: any) {
    this.global_utilities.displayAlertDialog({
      data: {
        title: "Abort File Upload?",
        message:
          "Upload is already in progress. Aborting now might lead to data inconsistencies.",
        dismiss_text: "Dismiss",
        action_text: "Abort",
        action: () => {
          cancel_method();
        },
      },
    });
  }

  cancelFile(index: number) {
    this.selected_files.splice(index, 1);
  }

  initiateCancelAll() {
    let selected_file_count = this.selected_files.length;
    let is_any_file_being_uploaded = false;
    for (let i = 0; i < selected_file_count; i++) {
      let selected_file = this.selected_files[i];
      // Checking if the file is being uploaded
      if (selected_file.is_upload_in_progress) {
        is_any_file_being_uploaded = true;
        break;
      }
    }
    if (is_any_file_being_uploaded) {
      this.displayFileUploadAbortConfirmation(() => {
        this.cancelAll();
      });
    } else {
      this.cancelAll();
    }
  }

  cancelAll() {
    this.global_utilities.scrollToElement(".modhyobitto-file-uploader", 100);
    let selected_file_count = this.selected_files.length;
    for (let i = 0; i < selected_file_count; i++) {
      this.selected_files.splice(0, 1);
    }
  }

  isAnyFileNotUploaded() {
    let selected_file_count = this.selected_files.length;
    let is_any_file_not_uploaded = false;
    for (let i = 0; i < selected_file_count; i++) {
      let selected_file = this.selected_files[i];
      // Checking if the file can be uploaded
      if (
        !selected_file.is_upload_in_progress &&
        selected_file.upload_result != "success"
      ) {
        is_any_file_not_uploaded = true;
        break;
      }
    }
    return is_any_file_not_uploaded;
  }

  ngOnDestroy(): void {
    this.global_utilities.unsubscribeAll([
      this.file_selection_sub,
      this.file_upload_sub,
    ]);
  }
}
