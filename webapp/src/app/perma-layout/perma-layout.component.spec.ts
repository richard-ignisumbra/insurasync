import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { PermaLayoutComponent } from "./perma-layout.component";

describe("PermaLayoutComponent", () => {
  let component: PermaLayoutComponent;
  let fixture: ComponentFixture<PermaLayoutComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PermaLayoutComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PermaLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
