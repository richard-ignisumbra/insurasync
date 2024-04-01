import { BehaviorSubject, Observable, Subject } from "rxjs";

import { Application } from "../../../../core/api/v1/model/application";
import { ApplicationSection } from "../../../../core/api/v1/model/applicationSection";
import { ApplicationSectionsService } from "../../../../core/api/v1/api/applicationSections.service";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class ApplicationService {
  private Sections = new BehaviorSubject<ApplicationSection[]>(null);
  Sections$: Observable<ApplicationSection[]> = this.Sections.asObservable();
  private activeSectionId: number;
  public constructor(
    private applicationSectionService: ApplicationSectionsService
  ) {}
  public setSections(newSections: ApplicationSection[]) {
    if (this.activeSectionId) {
      const activeSection = newSections.find(
        (section) => section.applicationSectionId === this.activeSectionId
      );
      if (activeSection) {
        activeSection.isActive = true;
      }
    }
    this.Sections.next(newSections);

    console.log("setting sections", newSections, this.activeSectionId);
  }
  public setActiveSection(sectionId: number) {
    const sections = this.Sections.getValue();
    this.activeSectionId = sectionId;
    console.log("setActiveSection", sectionId, sections);
    if (sections) {
      const activeSection = sections.find(
        (section) => section.applicationSectionId === sectionId
      );
      const oldSections = sections.filter(
        (section) => section.applicationSectionId !== sectionId
      );
      for (const section of oldSections) {
        section.isActive = false;
      }

      if (activeSection) {
        activeSection.isActive = true;
      }
      this.setSections(sections);
    }
  }
  public allSectionsCompleted(): boolean {
    const sections = this.Sections.getValue();
    let allCompleted = true;
    for (const section of sections) {
      if (section.isCompleted === false && allCompleted === true) {
        allCompleted = false;
      }
    }
    return allCompleted;
  }
  public async CompleteSection(
    sectionId: number,
    applicationId: number,
    isCompleted: boolean
  ) {
    const sections = this.Sections.getValue();
    console.log("trying to complete", sections);
    this.applicationSectionService
      .apiApplicationSectionsApplicationIdSectionIdCompletePost(
        applicationId,
        sectionId,
        isCompleted
      )
      .subscribe((e) => {});
    if (sections) {
      const completedSection = sections.find(
        (section) => section.applicationSectionId === sectionId
      );
      if (completedSection) {
        completedSection.isCompleted = isCompleted;
        console.log("set completed", completedSection);
      } else {
        console.log("section not found", sectionId);
      }
      this.setSections(sections);
    } else {
      console.error("sections not found");
    }
  }
}
