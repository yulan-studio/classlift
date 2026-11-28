from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH

out = r"C:\Users\yulan\source2\repos\classlift\classlift_management\Docs\Stripe Checkout Session Completed and Invoice Paid Events.docx"
doc = Document()
section = doc.sections[0]
section.top_margin = Inches(0.7)
section.bottom_margin = Inches(0.7)
section.left_margin = Inches(0.8)
section.right_margin = Inches(0.8)
doc.styles['Normal'].font.name = 'Aptos'
doc.styles['Normal'].font.size = Pt(10.5)
for style_name in ['Title', 'Heading 1', 'Heading 2']:
    doc.styles[style_name].font.color.rgb = RGBColor(0, 0, 0)

title = doc.add_paragraph(style='Title')
title.alignment = WD_ALIGN_PARAGRAPH.LEFT
title.add_run('Stripe Checkout Session Completed and Invoice Paid Events')
subtitle = doc.add_paragraph('Webhook event responsibilities and order-independent processing for Plan and Add-on subscriptions.')
subtitle.runs[0].italic = True

def heading(text, level=1):
    doc.add_heading(text, level=level)

def paragraph(text):
    doc.add_paragraph(text)

heading('Purpose')
paragraph('This document explains the difference between Stripe checkout.session.completed and invoice.paid, why their arrival order cannot be assumed, and how ClassLift should process both events safely for Plan and Add-on subscriptions.')
heading('The difference between the two events')
paragraph('checkout.session.completed means that the customer completed a Stripe Checkout flow. For a subscription Checkout, it is used to obtain the Stripe Subscription ID and establish the local subscription relationship.')
paragraph('invoice.paid means that a specific Stripe Invoice was successfully paid. It is used to record the bill and payment, update the billing period, and keep the local subscription active.')
table = doc.add_table(rows=1, cols=3)
table.style = 'Table Grid'
for cell, text in zip(table.rows[0].cells, ['Event', 'Business meaning', 'Main local work']):
    cell.text = text
    cell.paragraphs[0].runs[0].bold = True
for row in [
    ('checkout.session.completed', 'Checkout flow completed', 'Create or activate the local subscription and save StripeSubscriptionId'),
    ('invoice.paid', 'One specific invoice was paid', 'Create or update the local invoice and payment record, then update billing status'),
]:
    cells = table.add_row().cells
    for cell, text in zip(cells, row): cell.text = text
heading('Why both events matter for the first subscription payment')
paragraph('For the first subscription purchase, both events may be related to the same customer action: the customer completes Checkout, Stripe creates the subscription, Stripe creates the first invoice, and the first invoice is paid.')
paragraph('Webhook delivery is asynchronous. Stripe does not guarantee that the application will receive these events in the same order as the underlying operations. The application may receive checkout.session.completed first, invoice.paid first, or the same event more than once after a retry.')
heading('Later billing cycles')
paragraph('The customer does not open a new Checkout Session for each renewal. Later cycles normally produce invoice.paid or invoice.payment_failed. There is normally no new checkout.session.completed event for each monthly renewal.')
heading('ClassLift processing design')
paragraph('Plan and Add-on subscriptions use the same event model but separate local records.')
for item in ['Plan: checkout.session.completed -> organization_subscriptions; invoice.paid -> invoices and payments', 'Add-on: checkout.session.completed -> organization_addon_subscriptions; invoice.paid -> addon_invoices and addon_payments']:
    doc.add_paragraph(item, style='List Bullet')
paragraph('The billing_scope metadata separates the workflows: billing_scope=plan and billing_scope=addon. The Add-on workflow also uses addon_key, while Plan uses plan_id and pending_plan_change_id.')
heading('How to handle either arrival order')
for title_text, body in [
    ('1. Save a pending local request before Checkout', 'Create PendingPlanChange with status PendingPayment before redirecting to Stripe. This gives invoice.paid a local record even if it arrives before checkout.session.completed.'),
    ('2. Make both handlers idempotent', 'Use StripeSubscriptionId for subscriptions, StripeInvoiceId for invoices, and ProviderTransactionId for payments. Existing records must be updated rather than inserted again.'),
    ('3. Do not activate from the browser redirect', 'The success URL is not proof of a verified payment. Activation and payment recording must be performed by the signed Stripe Webhook.'),
    ('4. Allow late events to complete missing work', 'If checkout.session.completed arrives first, it creates the subscription and invoice.paid adds the invoice and payment. If invoice.paid arrives first, it uses the Stripe Subscription ID and pending metadata to create missing local records.'),
    ('5. Retry safely', 'Webhook retries must update existing records. Stable Stripe IDs and unique indexes prevent duplicates.'),
]:
    heading(title_text, 2)
    paragraph(body)
heading('Expected final state')
paragraph('Plan: PendingPlanChange.Status=Paid; OrganizationSubscription.Status=Active; StripeSubscriptionId is populated; Invoice.InvoiceStatus=Paid; Payment.PaymentStatus=Succeeded.')
paragraph('Add-on: OrganizationAddOnSubscription.Status=Active; StripeSubscriptionId is populated; AddOnInvoice.Status=Paid; AddOnPayment.PaymentStatus=Succeeded.')
heading('Operational verification')
for item in ['Confirm checkout.session.completed contains the expected billing_scope and identifiers.', 'Confirm invoice.paid contains the same Stripe Subscription ID.', 'Confirm the Webhook endpoint returns HTTP 2xx.', 'Confirm local subscription, invoice, and payment records exist.', 'Resend either event and verify that record counts do not increase unexpectedly.']:
    doc.add_paragraph(item, style='List Number')
heading('Summary')
paragraph('checkout.session.completed establishes the Checkout-to-Subscription relationship. invoice.paid records each successful billing transaction. They are related but not interchangeable. Pending records, stable Stripe IDs, unique indexes, and idempotent upsert processing ensure that either event order produces the same final local state.')
doc.save(out)
print(out)
