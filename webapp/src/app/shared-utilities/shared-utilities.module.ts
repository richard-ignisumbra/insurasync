import { AppMaterialImporterModule } from "../app-material-importer.module";
import { CommonModule } from "@angular/common";
import { DragAndDropDirective } from "./DragandDropDirective";
import { MathisDialogComponent } from "./mathis-dialog/mathis-dialog.component";
import { MathisFileUploaderComponent } from "./shared-components/mathis-uploader/mathis-uploader.component";
import { MemberFileUploaderComponent } from "./shared-components/member-uploader/member-uploader.component";
import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";

const UTILITY_COMPONENTS = [
  MathisDialogComponent,
  MathisFileUploaderComponent,
  MemberFileUploaderComponent,
  DragAndDropDirective,
];

@NgModule({
  imports: [CommonModule, ReactiveFormsModule, AppMaterialImporterModule],
  declarations: UTILITY_COMPONENTS,
  exports: UTILITY_COMPONENTS,
})
export class SharedUtilitiesModule {}
