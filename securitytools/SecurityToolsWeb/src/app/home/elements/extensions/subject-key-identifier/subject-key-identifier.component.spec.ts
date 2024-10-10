import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectKeyIdentifierComponent } from './subject-key-identifier.component';

describe('SubjectKeyIdentifierComponent', () => {
  let component: SubjectKeyIdentifierComponent;
  let fixture: ComponentFixture<SubjectKeyIdentifierComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubjectKeyIdentifierComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubjectKeyIdentifierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
