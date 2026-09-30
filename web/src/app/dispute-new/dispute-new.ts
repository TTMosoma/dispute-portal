import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-dispute-new',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './dispute-new.html',
  styleUrl: './dispute-new.css',
})
export class DisputeNewComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  transactionId = '';
  category = 'UnauthorizedTransaction';
  reason = '';
  loading = signal(false);
  error = signal<string | null>(null);

  categories = ['UnauthorizedTransaction', 'IncorrectAmount', 'DuplicateCharge', 'GoodsNotReceived', 'Other'];

  ngOnInit() {
    this.transactionId = this.route.snapshot.queryParamMap.get('transactionId') ?? '';
  }

  submit() {
    this.loading.set(true);
    this.error.set(null);
    this.http.post('/api/disputes', {
      transactionId: this.transactionId,
      category: this.category,
      reason: this.reason,
    }).subscribe({
      next: () => this.router.navigate(['/disputes']),
      error: (e) => {
        this.error.set(e.status === 409 ? 'An active dispute already exists for this transaction.' : 'Could not raise dispute.');
        this.loading.set(false);
      },
    });
  }
}
