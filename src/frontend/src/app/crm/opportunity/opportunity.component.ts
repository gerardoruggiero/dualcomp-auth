import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-opportunity',
  imports: [],
  template: `<p>opportunity works!</p>`,
  styleUrl: './opportunity.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OpportunityComponent { }
